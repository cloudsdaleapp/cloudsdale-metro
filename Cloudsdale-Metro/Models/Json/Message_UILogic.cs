using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Cloudsdale.Models.Json {
    public partial class Message {

        public static readonly Regex LinkRegex = new Regex(@"\bhttp(s)?\://([^ \,\:\;\""\']+)");

        public RichTextBlock Lines {
            get {

                // Creates a richtextblock out of the messages contained within

                // Start with an RTB
                var block = new RichTextBlock {
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 18,
                };
                // Add all the messages to the rich text block
                foreach (var msg in _subMessages) {
                    // Create a paragraph to store all of the inlines generated for the lines
                    var para = new Paragraph();
                    // Parse out string literals because zee's crazy server gives me doubly-escaped strings >_>
                    var txt = Helpers.ParseLiteral(msg.Content);
                    // Remove carraige returns and split at the linebreaks into the various lines in this message
                    var split = txt.Replace("\r", "").Split('\n');
                    for (var i = 0; i < split.Length; i++) {
                        // Parse the line into the paragraph, and as long as it isn't the last line add a linebreak.
                        var line = split[i];
                        AddInlinesWithLinks(para, line);
                        if (i < split.Length - 1)
                            para.Inlines.Add(new LineBreak());
                    }
                    // Finally, add the paragraph to the block
                    block.Blocks.Add(para);
                }
                return block;
            }
        }

        static void AddInlinesWithLinks(Paragraph para, string line) {
            // If the line starts with gt, it should be green. Otherwise, black.
            var color = new SolidColorBrush(
                line.StartsWith(">") ? Colors.Green : Colors.Black);
            if (LinkRegex.IsMatch(line)) {
                // If the line contains a link, parse it with extra link-handling goodness
                ParseLink(para, line, color);
            } else {
                // If it doesn't have a link, just add the text to the paragraph
                AddText(para, line, color);
            }
        }

        static void ParseLink(Paragraph para, string line, Brush color) {
            // As long as I can keep finding links, keep parsing them
            while (LinkRegex.IsMatch(line)) {
                // Get the match so I can disect it's goodies >:3
                var match = LinkRegex.Match(line);
                
                // Add the part of the text before the link to the paragraph
                AddText(para, line.Substring(0, match.Index), color);

                // Add the link to the paragraph
                AddLink(para, match.Value);

                // reset the line to the part after the link
                line = line.Substring(match.Length + match.Index);
            }

            // Add the remaining text after the last link to the paragraph
            AddText(para, line, color);
        }

        static void AddText(Paragraph para, string text, Brush color) {
            // Just make a simple run from the trimmed text and color and add it to the paragraph
            para.Inlines.Add(new Run {
                Text = text.Trim(),
                Foreground = color
            });
        }

        static void AddLink(Paragraph para, string linktext) {
            // Creates a hyperlink object
            var link = new HyperlinkButton {
                // Margins to counteract natural size and bring it on par with the line size
                Margin = new Thickness(-3, -10, -3, -9),
                // Set it's text to the URL
                Content = linktext,
                // Gives it a foreground of cornflower blue
                Foreground = new SolidColorBrush(Colors.DarkBlue),
                // Makes the font size 16 like the rest of the chat
                FontSize = 18,
                // Sets the padding to 0 to counteract even more margin-creep
                Padding = new Thickness(0),
            };
            // Launches the webbrowser with the given URL when the link is clicked
            link.Click += (sender, args) => Launcher.LaunchUriAsync(new Uri(linktext));
            // Adds the hyperlink to the paragraph with a ui object container
            para.Inlines.Add(new InlineUIContainer {
                Child = link,
            });
        }

        private readonly List<Message> _subMessages;
        public void AddSubMessage(Message message) {
            if (_subMessages.Any(m => m.Id == message.Id)) return;

            _subMessages.Add(message);
            _subMessages.Sort(new BaseComparer());
            OnPropertyChanged("Lines");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(Message other) {
            return other == null ? 1 : EndTime.CompareTo(other.EndTime);
        }

        public string TimeString {
            get {
                var tzi = TimeZoneInfo.Local.BaseUtcOffset + new TimeSpan(1, 0, 0);
                return ((TimeStamp ?? new DateTime(0)) + tzi).ToString("t");
            }
        }

        public class BaseComparer : IComparer<Message> {
            public int Compare(Message x, Message y) {
                return (x.TimeStamp ?? new DateTime()).CompareTo(y.TimeStamp ?? new DateTime());
            }
        }

        private SolidColorBrush _bgb;
        public Brush BackgroundBrush {
            get { return _bgb ?? (_bgb = new SolidColorBrush(Colors.CornflowerBlue)); }
        }

        private static readonly Random Rand = new Random();
        static Color RandColor() {
            return Color.FromArgb(0xFF,
                (byte)Rand.Next(0x7F, 0xFF),
                (byte)Rand.Next(0x7F, 0xFF),
                (byte)Rand.Next(0x7F, 0xFF));
        }
    }
}
