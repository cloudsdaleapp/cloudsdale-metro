using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;
using MetroFayeClient;
using MetroFayeClient.FayeObjects;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace Cloudsdale.Controllers.Data {
    public static class ConnectionController {
        #region Members
        public static FayeConnector Faye;
        private static LoggedInUser _currentUser;
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

        static async void FayeMessageReceived(FayeConnector sender, FayeMessageEventArgs e) {
            var chansplit = e.Channel.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (chansplit.Length < 3) return;
            if (chansplit[0] != "clouds") return;
            CloudProcessor processor;
            if (CloudProcessors.ContainsKey(chansplit[1])) {
                processor = GetProcessor(chansplit[1]);
            } else {
                return;
            }
            switch (chansplit[2]) {
                case "chat":
                    var message = await Helpers.DeserializeAsync<ChannelMessage<Message>>(e.Data);
                    processor.MessageProcessor.Add(message.Data);
                    break;
                case "users":
                    var user = await Helpers.DeserializeAsync<ChannelMessage<User>>(e.Data);
                    processor.UserProcessor.Heartbeat(UserProcessor.RegisterData(user.Data));
                    break;
                default:
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
        public static async Task<CloudProcessor> Subscribe(string cloud) {
            CloudProcessor processor;
            if (CloudProcessors.ContainsKey(cloud)) {
                processor = CloudProcessors[cloud];
            } else {
                processor = CloudProcessors[cloud] = new CloudProcessor();
            }
            if (Faye.IsSubscribed(("/clouds/{id}/users".Replace("{id}", cloud)))) {
                return processor;
            }
            Debug.WriteLine("Starting subscribe of chat");
            await Faye.Subscribe("/clouds/{id}/chat/messages".Replace("{id}", cloud));
            Debug.WriteLine("Starting subscribe of drops");
            await Faye.Subscribe("/clouds/{id}/drops".Replace("{id}", cloud));
            Debug.WriteLine("Starting subscribe of users");
            await Faye.Subscribe("/clouds/{id}/users".Replace("{id}", cloud));
            var messages = await WebData.GetDataAsync<Message[]>(WebserviceItem.Messages, cloud);
            foreach (var message in messages.Data) processor.MessageProcessor.Add(message);
            return processor;
        }

        public static CloudProcessor GetProcessor(string cloud) {
            return CloudProcessors[cloud];
        }
        #endregion
    }
}
