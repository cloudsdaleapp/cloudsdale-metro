using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MetroFaye {
    public delegate void FayeCallback(MessageHandler handler, JObject response);
    public abstract class MessageHandler {
        private readonly JObject extensionData = new JObject();
        private readonly DateTime creationDate = new DateTime();

        public event FayeCallback Handshaked;
        public event FayeCallback Subscribed;
        public event FayeCallback Unsubscribed;
        public event FayeCallback Published;
        public event FayeCallback MessageReceived;
        public event Action Disconnected;

        public event EventHandler<JObject> HandshakedEvent;
        public event EventHandler<JObject> SubscribedEvent;
        public event EventHandler<JObject> UnsubscribedEvent;
        public event EventHandler<JObject> PublishedEvent;
        public event EventHandler<JObject> MessageReceivedEvent;

        public virtual JObject ExtensionData { get { return extensionData; } }
        public abstract string ClientId { get; }
        public abstract bool IsConnecting { get; }
        public abstract bool IsConnected { get; }
        public IMessageReciever PrimaryReciever { get; set; }
        public DateTime CreationDate { get { return creationDate; } }

        internal Uri Address;

        public abstract bool IsSubscribed(string channel);

        public abstract void Connect();
        public abstract Task ConnectAsync();

        public abstract void Subscribe(string channel);
        public abstract void Unsubscribe(string channel);
        public abstract void Publish(string channel, JObject data);

        protected virtual void HandshakeCallback(JObject message) { }
        protected virtual void ConnectCallback(JObject message) { }
        protected virtual void SubscribeCallback(JObject message) { }
        protected virtual void UnsubscribeCallback(JObject message) { }
        protected virtual void PublishCallback(JObject message) { }

        protected void ProcessMessage(JToken message) {
            var channel = (string)message["channel"];
            var chansplit = channel.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (chansplit.Length < 2) return;

            if (chansplit[0] == "meta") {
                switch (chansplit[1]) {
                    case "handshake":
                        HandshakeCallback((JObject)message);
                        break;
                    case "connect":
                        ConnectCallback((JObject)message);
                        break;
                    case "subscribe":
                        SubscribeCallback((JObject)message);
                        break;
                    case "unsubscribe":
                        UnsubscribeCallback((JObject)message);
                        break;
                }
                return;
            }

            if (message["successful"] != null) {
                PublishCallback((JObject)message);
            }

            if (PrimaryReciever != null) {
                PrimaryReciever.OnMessage((JObject)message);
            }
            OnReceive((JObject)message);
        }

        protected void OnHandshaked(JObject data) {
            var handler = Handshaked;
            if (handler != null) handler(this, data);
            var handlerE = HandshakedEvent;
            if (handlerE != null) handlerE(this, data);
        }
        protected void OnSubscribed(JObject data) {
            var handler = Subscribed;
            if (handler != null) handler(this, data);
            var handlerE = SubscribedEvent;
            if (handlerE != null) handlerE(this, data);
        }
        protected void OnUnsubscribed(JObject data) {
            var handler = Unsubscribed;
            if (handler != null) handler(this, data);
            var handlerE = UnsubscribedEvent;
            if (handlerE != null) handlerE(this, data);
        }
        protected void OnPublished(JObject data) {
            var handler = Published;
            if (handler != null) handler(this, data);
            var handlerE = PublishedEvent;
            if (handlerE != null) handlerE(this, data);
        }
        protected void OnReceive(JObject data) {
            var handler = MessageReceived;
            if (handler != null) handler(this, data);
            var handlerE = MessageReceivedEvent;
            if (handlerE != null) handlerE(this, data);
        }
        protected void OnDisconnect() {
            var handler = Disconnected;
            if (handler != null) handler();
        }
    }
}
