using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CloudsdaleLib;
using CloudsdaleLib.Annotations;
using CloudsdaleLib.Helpers;
using CloudsdaleLib.Models;
using CloudsdaleLib.Providers;
using Cloudsdale_Metro.Models;
using MetroFaye;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cloudsdale_Metro.Controllers {
    public class CloudController : IStatusProvider, IMessageReciever, INotifyPropertyChanged {
        private int _unreadMessages;
        private readonly Dictionary<string, Status> userStatuses = new Dictionary<string, Status>();
        private readonly ModelCache<Message> messages = new ModelCache<Message>(50);
        private DateTime? _validatedFayeClient;

        public CloudController(Cloud cloud) {
            Cloud = cloud;
        }

        public Cloud Cloud { get; private set; }

        public ModelCache<Message> Messages { get { return messages; } }

        public async Task EnsureLoaded() {
            if (_validatedFayeClient == null || _validatedFayeClient < App.Connection.Faye.CreationDate) {
                App.Connection.Faye.Subscribe("/clouds/" + Cloud.Id + "/users/*");
            }

            await Cloud.Validate();
            var client = new HttpClient { DefaultRequestHeaders = { { "Accept", "application/json" } } };
            var response = await client.GetStringAsync(Endpoints.CloudMessagesEndpoint.Replace("[:id]", Cloud.Id));
            var responseMessages = await JsonConvert.DeserializeObjectAsync<WebResponse<Message[]>>(response);
            var newMessages = new List<Message>(messages.Where(message => message.Timestamp > responseMessages.Result.Last().Timestamp));
            messages.Clear();
            foreach (var message in responseMessages.Result) {
                messages.AddToEnd(message);
            }
            foreach (var message in newMessages) {
                messages.AddToEnd(message);
            }

            _validatedFayeClient = App.Connection.Faye.CreationDate;
        }

        public void OnMessage(JObject message) {
            var chanSplit = ((string)message["channel"]).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (chanSplit.Length == 4 && chanSplit[2] == "chat" && chanSplit[3] == "messages") {
                OnChatMessage(message["data"]);
            }
        }

        private void OnChatMessage(JToken message) {
            AddUnread();
            var messageModel = message.ToObject<Message>();

            if (messageModel.ClientId == App.Connection.Faye.ClientId) return;

            messageModel.Author.CopyTo(messageModel.User);
            messages.AddToEnd(messageModel);
        }

        private void AddUnread() {
            ++UnreadMessages;
            if (App.Connection.MessageController.CurrentCloud == this) {
                UnreadMessages = 0;
            }
        }

        public int UnreadMessages {
            get { return _unreadMessages; }
            private set {
                if (value == _unreadMessages) return;
                _unreadMessages = value;
                OnPropertyChanged();
            }
        }

        public Status StatusForUser(string userId) {
            return userStatuses.ContainsKey(userId) ? Status.Offline : userStatuses[userId] = Status.Offline;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
