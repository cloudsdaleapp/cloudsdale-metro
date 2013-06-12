using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cloudsdale_Metro.Helpers;
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
    public sealed partial class HyperRichTextBlock {
        public HyperRichTextBlock() {
            InitializeComponent();
        }

        public BlockCollection Blocks {
            get { return RichText.Blocks; }
        }

        #region Text Alignment

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(HyperRichTextBlock),
            new PropertyMetadata(TextAlignment.Left, AlignmentChanged));

        private static void AlignmentChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) {
            var control = (HyperRichTextBlock)dependencyObject;
            control.RichText.TextAlignment = (TextAlignment)args.NewValue;
        }

        public TextAlignment TextAlignment {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        #endregion

        private void RichText_OnTapped(object sender, TappedRoutedEventArgs e) {
            var pointer = RichText.GetPositionFromPoint(e.GetPosition(RichText));

            var link = pointer.ScaleFor<Hyperlink>();
            if (link != null) {
                link.TriggerLink(RichText);
            }
        }
    }
}
