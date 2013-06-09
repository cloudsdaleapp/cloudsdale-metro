using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Cloudsdale_Metro.Views.Controls {
    public sealed partial class MessageTextControl {
        public static readonly Regex Greentext = new Regex(@"^\>");

        public MessageTextControl() {
            InitializeComponent();
        }

        public static readonly DependencyProperty MessagesProperty = DependencyProperty.Register("Messages",
            typeof(string[]), typeof(MessageTextControl),
            new PropertyMetadata(default(string[]), MessagesChanged));

        private static void MessagesChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) {
            var control = (MessageTextControl)dependencyObject;

            control.UpdateContents();
        }

        public string[] Messages {
            get { return (string[])GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        private void UpdateContents() {
            RichText.Blocks.Clear();
            foreach (var message in Messages) {
                var block = new Paragraph {
                    Margin = new Thickness(5),
                };
                foreach (var inline in ParseMessage(message)) {
                    block.Inlines.Add(inline);
                }
                RichText.Blocks.Add(block);
            }
        }

        private IEnumerable<Inline> ParseMessage(string message) {
            var first = true;
            foreach (var line in message.Split('\n')) {
                if (!first) {
                    yield return new LineBreak();
                }
                yield return new Run {
                    Text = line,
                    Foreground = new SolidColorBrush(Greentext.IsMatch(line) ? Colors.MediumSeaGreen : Colors.Black)
                };
                first = false;
            }
        } 
    }

    class TextGroup {
        public string Text;
        public Inline Inline;
    }
}
