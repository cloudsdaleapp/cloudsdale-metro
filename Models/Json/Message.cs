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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
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
        public DateTime? TimeStamp { get; set; }
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

        public RichTextBlock[] Lines {
            get {
                var block = new RichTextBlock {
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 24,
                };
                foreach (var msg in _subMessages) {
                    var para = new Paragraph();
                    var txt = Helpers.ParseLiteral(msg.Content);
                    var split = txt.Replace("\r", "").Split('\n');
                    foreach (var line in split) {
                        para.Inlines.Add(new Run{Text = line});
                    }
                    block.Blocks.Add(para);
                }
                return new[] {block};
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

        public int CompareTo(Message other) {
            return other == null ? 1 : (TimeStamp ?? new DateTime(0)).CompareTo(other.TimeStamp ?? new DateTime(0));
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Topic : CloudsdaleItem {
        [JsonProperty("type")]
        public string Type;
    }
}
