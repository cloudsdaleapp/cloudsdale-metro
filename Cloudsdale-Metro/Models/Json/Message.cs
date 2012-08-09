using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cloudsdale.Controllers.Data;
using Newtonsoft.Json;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
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

        public static readonly Regex LinkRegex = new Regex(@"\bhttp(s)?\://([^ \,\:\;\""\']+)");

        public RichTextBlock[] Lines {
            get {
                var block = new RichTextBlock {
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 16,
                };
                foreach (var msg in _subMessages) {
                    var para = new Paragraph();
                    var txt = Helpers.ParseLiteral(msg.Content);
                    var split = txt.Replace("\r", "").Split('\n');
                    for (var i = 0; i < split.Length; i++) {
                        var line = split[i];
                        var thislinecolor = new SolidColorBrush(
                            line.StartsWith(">") ? Colors.Green : Colors.Black);
                        if (LinkRegex.IsMatch(line)) {
                            var remaining = line;
                            while (LinkRegex.IsMatch(remaining)) {
                                var match = LinkRegex.Match(remaining);
                                para.Inlines.Add(new Run {
                                    Text = remaining.Substring(0, match.Index),
                                    Foreground = thislinecolor
                                });
                                var link = new HyperlinkButton {
                                    Margin = new Thickness(0, -10, 0, -10),
                                    Content = match.Value,
                                    Foreground = new SolidColorBrush(Colors.CornflowerBlue),
                                    FontSize = 16,
                                    Padding = new Thickness(0),
                                };
                                link.Click += (sender, args) => Launcher.LaunchUriAsync(new Uri(match.Value));
                                para.Inlines.Add(new InlineUIContainer {
                                    Child = link,
                                });

                                remaining = remaining.Substring(match.Length + match.Index);
                            }
                            para.Inlines.Add(new Run {
                                Text = remaining,
                                Foreground = thislinecolor
                            });
                        } else {
                            para.Inlines.Add(new Run {
                                Text = line,
                                Foreground = thislinecolor
                            });
                        }
                        if (i < split.Length - 1)
                            para.Inlines.Add(new LineBreak());
                    }
                    block.Blocks.Add(para);
                }
                return new[] { block };
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

        public string TimeString {
            get { return (TimeStamp ?? new DateTime(0)).ToString("t"); }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Topic : CloudsdaleItem {
        [JsonProperty("type")]
        public string Type;
    }
}
