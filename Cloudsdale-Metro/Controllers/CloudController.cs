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
            userStatuses[App.Connection.Session.CurrentSession.Id] =
                App.Connection.Session.CurrentSession.PreferredStatus;
        }

        public Cloud Cloud { get; private set; }

        public ModelCache<Message> Messages { get { return messages; } }

        public List<User> OnlineModerators {
            get {
                var list =
                    userStatuses.Where(kvp => kvp.Value != Status.offline)
                                .Where(kvp => Cloud.ModeratorIds.Contains(kvp.Key))
                                .Select(kvp => App.Connection.ModelController.GetUser(kvp.Key))
                                .ToList();
                list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
                return list;
            }
        }

        public List<User> AllModerators {
            get {
                var list =
                    Cloud.ModeratorIds
                                .Select(mid => App.Connection.ModelController.GetUser(mid))
                                .ToList();
                list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
                return list;
            }
        }

        public List<User> OnlineUsers {
            get {
                var list =
                    userStatuses.Where(kvp => kvp.Value != Status.offline)
                                .Where(kvp => Cloud.UserIds.Contains(kvp.Key))
                                .Where(kvp => !Cloud.ModeratorIds.Contains(kvp.Key))
                                .Select(kvp => App.Connection.ModelController.GetUser(kvp.Key))
                                .ToList();
                list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
                return list;
            }
        }
        public List<User> AllUsers {
            get {
                var list =
                    userStatuses.Where(kvp => Cloud.UserIds.Contains(kvp.Key))
                                .Where(kvp => !Cloud.ModeratorIds.Contains(kvp.Key))
                                .Select(kvp => App.Connection.ModelController.GetUser(kvp.Key))
                                .ToList();
                list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
                return list;
            }
        }

        public async Task EnsureLoaded() {
            if (_validatedFayeClient == null || _validatedFayeClient < App.Connection.Faye.CreationDate) {
                App.Connection.Faye.Subscribe("/clouds/" + Cloud.Id + "/users/*");
            }

            await Cloud.Validate();

            var client = new HttpClient { DefaultRequestHeaders = { { "Accept", "application/json" } } };

            // Load user list
            {
                var response = await client.GetStringAsync((
                    Cloud.UserIds.Length > 100
                    ? Endpoints.CloudOnlineUsers
                    : Endpoints.CloudUsers)
                    .Replace("[:id]", Cloud.Id));
                var userData = await JsonConvert.DeserializeObjectAsync<WebResponse<User[]>>(response);
                var users = new List<User>();
                foreach (var user in userData.Result) {
                    if (user.Status != null) {
                        SetStatus(user.Id, (Status)user.Status);
                    }
                    users.Add(await App.Connection.ModelController.UpdateDataAsync(user));
                }

            }

            // Load messages
            {
                var response = await client.GetStringAsync(Endpoints.CloudMessages.Replace("[:id]", Cloud.Id));
                var responseMessages = await JsonConvert.DeserializeObjectAsync<WebResponse<Message[]>>(response);
                var newMessages = new List<Message>(messages
                    .Where(message => message.Timestamp > responseMessages.Result.Last().Timestamp));
                messages.Clear();
                foreach (var message in responseMessages.Result) {
                    StatusForUser(message.Author.Id);
                    messages.AddToEnd(message);
                }
                foreach (var message in newMessages) {
                    StatusForUser(message.Author.Id);
                    messages.AddToEnd(message);
                }
            }

            _validatedFayeClient = App.Connection.Faye.CreationDate;
        }

        public void OnMessage(JObject message) {
            var chanSplit = ((string)message["channel"]).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (chanSplit.Length == 4 && chanSplit[2] == "chat" && chanSplit[3] == "messages") {
                OnChatMessage(message["data"]);
            } else if (chanSplit.Length == 4 && chanSplit[2] == "users") {
                OnUserMessage(chanSplit[3], message["data"]);
            } else if (chanSplit.Length == 2) {
                OnCloudData(message["data"]);
            }
        }

        private void OnChatMessage(JToken message) {
            AddUnread();
            var messageModel = message.ToObject<Message>();

            if (messageModel.ClientId == App.Connection.Faye.ClientId) return;

            messageModel.Author.CopyTo(messageModel.User);
            messages.AddToEnd(messageModel);
        }

        private async void OnUserMessage(string id, JToken jUser) {
            jUser["id"] = id;
            var user = jUser.ToObject<User>();
            if (user.Status != null) {
                SetStatus(user.Id, (Status)user.Status);
            }
            await App.Connection.ModelController.UpdateDataAsync(user);
        }

        private void OnCloudData(JToken cloudData) {
            cloudData.ToObject<Cloud>().CopyTo(Cloud);
        }

        private void AddUnread() {
            ++UnreadMessages;
            if (App.Connection.MessageController.CurrentCloud == this) {
                UnreadMessages = 0;
            }
        }

        public int UnreadMessages {
            get { return _unreadMessages; }
            set {
                if (value == _unreadMessages) return;
                _unreadMessages = value;
                OnPropertyChanged();
            }
        }

        private Status SetStatus(string userId, Status status) {
            userStatuses[userId] = status;
            OnPropertyChanged("OnlineModerators");
            OnPropertyChanged("AllModerators");
            OnPropertyChanged("OnlineUsers");
            OnPropertyChanged("AllUsers");
            return status;
        }

        public Status StatusForUser(string userId) {
            return userStatuses.ContainsKey(userId) ? userStatuses[userId] : SetStatus(userId, Status.offline);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
