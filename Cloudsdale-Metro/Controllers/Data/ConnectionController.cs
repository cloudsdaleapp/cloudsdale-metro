using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;
using MetroFayeClient;
using MetroFayeClient.FayeObjects;
using Newtonsoft.Json;
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
            await Faye.Subscribe("/clouds/{id}/users".Replace("{id}", cloud.Id));
            return processor;
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
