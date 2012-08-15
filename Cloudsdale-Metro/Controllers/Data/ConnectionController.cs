using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;
using MetroFayeClient;
using MetroFayeClient.FayeObjects;
using Newtonsoft.Json;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Cloudsdale.Controllers.Data {
    public static class ConnectionController {
        #region Members
        public static FayeConnector Faye;
        private static LoggedInUser _currentUser;
        private static DateTime _lastMessage = DateTime.Now;
        public static event Action LostConnection;
        public static LoggedInUser CurrentUser {
            get { return _currentUser; }
            set {
                _currentUser = value;
                Application.Current.Resources["CurrentUser"] = value;
            }
        }
        public static Cloud CurrentCloud;
        private static readonly Dictionary<string, CloudProcessor> CloudProcessors =
                            new Dictionary<string, CloudProcessor>();
        public static List<string> CloudOrder = null;
        #endregion

        public static async Task<HandshakeResponse> Connect() {
            Faye = new FayeConnector();
            Faye.MessageReceived += FayeMessageReceived;
            return await Faye.ConnectAsync(new Uri("ws://push01.cloudsdale.org/push"));
        }

        public static async void ReceiveTimeout() {
            await Task.Delay(10000);
            if (_lastMessage < DateTime.Now.AddSeconds(-30)) {
                if (CurrentUser == null) return;
                if (CurrentUser.Clouds.Any(cloud => cloud.IsDataPreloaded)) {
                    if (LostConnection != null)
                        LostConnection();
                }
            }
        }

        public static async Task<bool> HandledUIConnect() {
            var tryagain = true;
            while (tryagain) {
                var fail = false;
                try {
                    var response = await Connect();
                    if (response != null && (response.Successful ?? false)) {
                        foreach (var cloud in CurrentUser.Clouds) {
                            Subscribe(cloud);
                        }
                        tryagain = false;
                    } else {
                        fail = true;
                    }
                } catch {
                    fail = true;
                }
                if (!fail) continue;
                var dialog = new MessageDialog("There was an error connecting to cloudsdale.");
                dialog.Commands.Clear();
                dialog.Commands.Add(new UICommand("Retry"));
                dialog.Commands.Add(new UICommand("Quit"));
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;
                if ((await dialog.ShowAsync()).Label != "Retry") {
                    Application.Current.Exit();
                    return false;
                }
            }
            return true;
        }

        static async void FayeMessageReceived(FayeConnector sender, FayeMessageEventArgs e) {
            _lastMessage = DateTime.Now;
            var chansplit = e.Channel.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (chansplit.Length < 3) return;
            if (chansplit[0] != "clouds") return;
            CloudProcessor processor;
            if (CloudProcessors.ContainsKey(chansplit[1])) {
                processor = CloudProcessors[chansplit[1]];
            } else {
                return;
            }
            switch (chansplit[2]) {
                case "chat":
                    var message = await Helpers.DeserializeAsync<ChannelMessage<Message>>(e.Data);
                    processor.MessageProcessor.Add(message.Data);
                    break;
                case "users":
                    var user = await Helpers.DeserializeAsync<ChannelMessage<ListUser>>(e.Data);
                    processor.UserProcessor.Heartbeat(user.Data);
                    break;
                case "drops":
                    var drop = await Helpers.DeserializeAsync<ChannelMessage<Drop>>(e.Data);
                    processor.DropProcessor.Add(drop.Data);
                    break;
            }
        }

        #region User
        public static async Task SaveUserAsync() {
            await Helpers.SaveAsGZippedJson(
                await Helpers.GetDataFileAsync("CurrentUser.gzip"),
                CurrentUser);
            CloudOrder = (from cloud in CurrentUser.Clouds
                          select cloud.Id).ToList();
            await Helpers.SaveAsGZippedJson(
                await Helpers.GetDataFileAsync("CloudOrder.gzip"),
                CloudOrder);
        }

        public static async Task<bool> LoadUserAsync() {
            if (await Helpers.DataFileExists("CloudOrder.gzip")) {
                var co = await Helpers.ReadGZippedJson<string[]>(
                             await Helpers.GetDataFileAsync("CloudOrder.gzip"));
                CloudOrder = new List<string>(co);
            } else if (CloudOrder == null) {
                CloudOrder = new List<string>();
            }
            if (await Helpers.DataFileExists("CurrentUser.gzip")) {
                CurrentUser = await Helpers.ReadGZippedJson<LoggedInUser>(
                              await Helpers.GetDataFileAsync("CurrentUser.gzip"));
                return true;
            }
            return false;
        }

        #endregion

        #region Messages, Drops, And Subscription
        public static async Task<CloudProcessor> Subscribe(Cloud cloud) {
            CloudProcessor processor;
            if (CloudProcessors.ContainsKey(cloud.Id)) {
                processor = CloudProcessors[cloud.Id];
            } else {
                processor = CloudProcessors[cloud.Id] = new CloudProcessor(cloud);
            }
            if (Faye.IsSubscribed(("/clouds/{id}/users".Replace("{id}", cloud.Id)))) {
                return processor;
            }
            Debug.WriteLine("Starting subscribe of chat");
            await Faye.Subscribe("/clouds/{id}/chat/messages".Replace("{id}", cloud.Id));
            Debug.WriteLine("Starting subscribe of drops");
            await Faye.Subscribe("/clouds/{id}/drops".Replace("{id}", cloud.Id));
            Debug.WriteLine("Starting subscribe of users");

            Broadcast(cloud);

            return processor;
        }

        public static bool IsSubscribed(string cloudid) {
            return Faye.IsSubscribed(("/clouds/{id}/users".Replace("{id}", cloudid)));
        }

        /// <summary>
        /// Begins broadcasting the user's presence into the cloud until the user is no longer subscribed
        /// </summary>
        static async void Broadcast(Cloud cloud) {
            // If the faye service is no longer connected, or if we are no longer subscribed stop the broadcast
            if (Faye == null || !Faye.Connected || !IsSubscribed(cloud.Id)) return;

            // Broadcasts the user's Id, Name, and Avatar into the users channel
            await Faye.Publish("/clouds/" + cloud.Id + "/users", new ListUser {
                Id = CurrentUser.Id,
                Name = CurrentUser.Name,
                Avatar = CurrentUser.Avatar,
            });

            // Waits 30 seconds and broadcasts again
            await Task.Delay(30000);
            Broadcast(cloud);
        }

        public static CloudProcessor GetProcessor(Cloud cloud) {
            return CloudProcessors.ContainsKey(cloud.Id) ? CloudProcessors[cloud.Id] :
                CloudProcessors[cloud.Id] = new CloudProcessor(cloud);
        }

        public static async Task SendMessage(Cloud cloud, string message) {
            var data = Encoding.UTF8.GetBytes(await Helpers.SerializeAsync(new SendMessageData {
                Content = message,
                ClientID = Faye.ClientID
            }));
            var request =
                WebRequest.CreateHttp("http://www.cloudsdale.org/v1/clouds/{id}/chat/messages"
                .Replace("{id}", cloud.Id));
            request.Accept = "application/json";
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["X-Auth-Token"] = CurrentUser.AuthToken;
            try {
                using (var requestStream = await request.GetRequestStreamAsync()) {
                    await requestStream.WriteAsync(data, 0, data.Length);
                    await requestStream.FlushAsync();
                }
                using (var response = await request.GetResponseAsync()) {
                    response.GetResponseStream().Dispose();
                }

            } catch (Exception e) {
                Debugger.Break();
            }
        }
        #endregion

        [JsonObject]
        public class SendMessageData {
            [JsonProperty("content")]
            public string Content;
            [JsonProperty("client_id")]
            public string ClientID;
        }
    }
}
