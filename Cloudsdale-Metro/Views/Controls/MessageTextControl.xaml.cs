using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Cloudsdale_Metro.Views.Controls {
    public sealed partial class MessageTextControl {
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
                var block = new Paragraph();
                block.Inlines.Add(new Run { Text = message });
                RichText.Blocks.Add(block);
            }
        }
    }

    class TextGroup {
        public string Text;
        public Inline Inline;
    }
}
