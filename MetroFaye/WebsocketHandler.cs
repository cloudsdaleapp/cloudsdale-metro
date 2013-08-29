using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace MetroFaye {
    internal class WebsocketHandler : MessageHandler {
        private string _clientId;
        private HandlerState _state;

        #region Implementation
        public override string ClientId {
            get { return _clientId; }
        }

        public override bool IsConnecting {
            get { return _state.connecting && !_state.connected && !_state.closed; }
        }

        public override bool IsConnected {
            get { return _state.connecting && _state.connected && !_state.closed; }
        }

        public override bool IsSubscribed(string channel) {
            return _state.subbedChannels.ToList().Any(chan => MatchChannel(chan, channel));
        }

        public override async Task ConnectAsync() {
            var handshakeWaiter = new EventWaiter();
            Handshaked += handshakeWaiter.Callback;

            await Connect(Address);
            await handshakeWaiter.Wait();

            Handshaked -= handshakeWaiter.Callback;
        }
        public override async void Connect() {
            await Connect(Address);
        }
        private async Task Connect(Uri address) {
            InitState();

            var wait = _state.socket.ConnectAsync(address);
            _state.connecting = true;
            await wait;
            _state.connected = true;

            var handshake = new JObject();
            handshake["channel"] = "/meta/handshake";
            handshake["version"] = "1.0";
            handshake["minimumVersion"] = "1.0";
            handshake["supportedConnectionTypes"] = JArray.FromObject(new[] { "websocket" });

            await SendNoId(handshake);
        }

        public override async void Subscribe(string channel) {
            var request = new JObject();
            request["channel"] = "/meta/subscribe";
            request["subscription"] = channel;

            await Send(request);
        }

        public override async void Unsubscribe(string channel) {
            var request = new JObject();
            request["channel"] = "/meta/unsubscribe";
            request["subscription"] = channel;

            await Send(request);
        }

        public override async void Publish(string channel, JObject data) {
            var request = new JObject();
            request["channel"] = channel;
            request["data"] = data;

            await Send(request);
        }
        #endregion

        #region Internals
        public async Task Send(JObject data) {
            data["clientId"] = ClientId;
            await SendNoId(data);
        }

        public async Task SendNoId(JObject data) {
            if (ExtensionData != null) {
                data["ext"] = ExtensionData;
            }

            using (var writer = new DataWriter(_state.socket.OutputStream)) {
                writer.UnicodeEncoding = UnicodeEncoding.Utf8;
                writer.WriteString(data.ToString());
                await writer.StoreAsync();
                await writer.FlushAsync();
                writer.DetachStream();
            }
        }

        public void InitState() {
            _state = new HandlerState {
                socket = new MessageWebSocket(),
                subbedChannels = new List<string>()
            };
            _state.socket.Closed += (sender, args) => {
                _state.closed = true;
                OnDisconnect();
            };
            _state.socket.MessageReceived += (sender, args) => {
                try {
                    JArray messages;
                    using (var reader = args.GetDataReader()) {
                        messages = JArray.Parse(reader.ReadString(reader.UnconsumedBufferLength));
                    }

                    foreach (var message in messages) {
                        ProcessMessage(message);
                    }
                } catch {
                    _state.socket.Dispose();
                    _state.closed = true;
                    OnDisconnect();
                }
            };
        }

        public bool MatchChannel(string pattern, string channel) {
            var psplit = pattern.Split('/');
            var csplit = channel.Split('/');

            if (csplit.Length < psplit.Length) return false;

            for (var i = 0; i < psplit.Length; ++i) {
                if (psplit[i] == "**") return true;
                if (psplit[i] != "*" && psplit[i] != csplit[i]) return false;
            }

            return csplit.Length == psplit.Length;
        }

        public async void ConnectRequest() {
            var request = new JObject();
            request["channel"] = "/meta/connect";
            request["connectionType"] = "websocket";

            await Send(request);
        }
        #endregion

        #region Callbacks
        protected override void HandshakeCallback(JObject message) {
            if (!(bool)message["successful"]) {
                _state.socket.Dispose();
                _state.closed = true;
                OnDisconnect();
            }

            _clientId = (string)message["clientId"];
            OnHandshaked(message);

            ConnectRequest();
        }

        protected override void ConnectCallback(JObject message) {
            ConnectRequest();
        }

        protected override void PublishCallback(JObject message) {
            OnPublished(message);
        }

        protected override void SubscribeCallback(JObject message) {
            if ((bool)message["successful"]) {
                _state.subbedChannels.Add((string)message["subscription"]);
            }
            OnSubscribed(message);
        }

        protected override void UnsubscribeCallback(JObject message) {
            _state.subbedChannels.Remove((string)message["subscription"]);
            OnUnsubscribed(message);
        }
        #endregion

        internal struct HandlerState {
            public MessageWebSocket socket;
            public bool connecting;
            public bool connected;
            public bool closed;

            public List<string> subbedChannels;
        }

        internal class EventWaiter {
            private readonly ManualResetEvent waiter = new ManualResetEvent(false);

            public async Task Wait() {
                await Task.Run(() => waiter.WaitOne());
            }

            public void Callback(MessageHandler handler, JObject response) {
                waiter.Set();
            }
        }
    }
}
