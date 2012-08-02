using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudsdale.Controllers.Data;
using Newtonsoft.Json;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Cloudsdale.Models.Json {
    [JsonObject(MemberSerialization.OptIn)]
    public class Message : CloudsdaleItem, INotifyPropertyChanged, IComparable<Message> {
        public Message() {
            _subMessages = new List<Message> {
                this
            };
        }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }
        [JsonProperty("content")]
        public string Content;
        [JsonProperty("user")]
        public User User {
            get { return _user; }
            set { _user = UserProcessor.RegisterData(value); }
        }

        private User _user;
        [JsonProperty("topic")]
        public Topic Topic;

        public ChatLine[] Lines {
            get {
                var lines = new List<ChatLine>();
                foreach (var msg in _subMessages) {
                    var txt = Helpers.ParseLiteral(msg.Content);
                    var split = txt.Replace("\r", "").Split('\n');
                    foreach (var line in split) {
                        lines.Add(new ChatLine(line, 
                            line.StartsWith(">") ? 
                            new SolidColorBrush(Colors.Green) : 
                            new SolidColorBrush(Colors.Black)));
                    }
                }
                return lines.ToArray();
            }
        }

        private readonly List<Message> _subMessages; 
        public void AddSubMessage(Message message) {
            _subMessages.Add(message);
            _subMessages.Sort();
            OnPropertyChanged("Lines");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public struct ChatLine {
            public ChatLine(string text, Brush brush) {
                _text = text;
                _brush = brush;
            }

            private readonly string _text;
            public string Text { get { return _text; } }
            private readonly Brush _brush;
            public Brush Brush {get { return _brush; }}
        }

        public int CompareTo(Message other) {
            return other == null ? 1 : TimeStamp.CompareTo(other.TimeStamp);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Topic : CloudsdaleItem {
        [JsonProperty("type")]
        public string Type;
    }
}
