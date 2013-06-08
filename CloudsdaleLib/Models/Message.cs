using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class Message : CloudsdaleModel, IMergable, IPreProcessable {
        public static readonly Regex SlashMeFormat = new Regex(@"^/me");

        public Message() {
            _drops = new Drop[0];
            _timestamp = DateTime.Now;
        }

        private string _id;
        private User _author;
        private Drop[] _drops;
        private string _authorId;
        private string _device;
        private string _clientId;
        private string _content;
        private DateTime _timestamp;

        private readonly List<Message> _messages = new List<Message>();

        [JsonProperty("id")]
        public string Id {
            get { return _id; }
            set {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("timestamp")]
        public DateTime Timestamp {
            get { return _timestamp; }
            set {
                if (value.Equals(_timestamp)) return;
                _timestamp = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("content")]
        public string Content {
            get { return _content; }
            set {
                if (value == _content) return;
                _content = value;
                OnPropertyChanged();
                OnPropertyChanged("Messages");
            }
        }

        [JsonProperty("client_id")]
        public string ClientId {
            get { return _clientId; }
            set {
                if (value == _clientId) return;
                _clientId = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("device")]
        public string Device {
            get { return _device; }
            set {
                if (value == _device) return;
                _device = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("author_id")]
        public string AuthorId {
            get { return _authorId; }
            set {
                if (value == _authorId) return;
                _authorId = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("drops")]
        public Drop[] Drops {
            get { return _drops; }
            set {
                if (Equals(value, _drops)) return;
                _drops = value;
                OnPropertyChanged();
                OnPropertyChanged("AllDrops");
            }
        }

        [JsonProperty("author")]
        public User Author {
            get { return _author; }
            set {
                if (Equals(value, _author)) return;
                _author = value;
                OnPropertyChanged();
                OnPropertyChanged("User");
            }
        }

        public string[] Messages {
            get {
                var messages = new string[_messages.Count + 1];
                messages[0] = Content;
                for (var i = 0; i < _messages.Count; ++i) {
                    messages[i + 1] = _messages[i].Content;
                }
                return messages;
            }
        }

        public IEnumerable<Drop> AllDrops {
            get {
                return _messages.Aggregate(new List<Drop>(Drops), (list, message) => {
                    list.AddRange(message.Drops);
                    return list;
                });
            }
        }

        public User User {
            get { return Cloudsdale.CloudServicesProvider.GetBackedUser(Author.Id); }
        }

        public void Merge(CloudsdaleModel other) {
            _messages.Add((Message)other);
            OnPropertyChanged("Messages");
            OnPropertyChanged("AllDrops");

            other.PropertyChanged += (sender, args) => OnPropertyChanged(args.PropertyName);
        }

        public bool CanMerge(CloudsdaleModel other) {
            var otherMessage = (Message) other;
            return User.Id == otherMessage.User.Id && !SlashMeFormat.IsMatch(Content) && !SlashMeFormat.IsMatch(otherMessage.Content);
        }

        public void PreProcess() {
            Content = Content.ParseMessage();
        }
    }
}
