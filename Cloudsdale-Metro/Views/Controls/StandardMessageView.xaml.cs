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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Cloudsdale_Metro.Views.Controls {
    public sealed partial class StandardMessageView {
        public StandardMessageView() {
            InitializeComponent();
        }

        private void StandardMessageView_OnSizeChanged(object sender, SizeChangedEventArgs e) {
            Separator.Width = e.NewSize.Width;

            if (e.NewSize.Width < 470) {
                DropGrid.Visibility = Visibility.Collapsed;
            } else {
                DropGrid.Visibility = Visibility.Visible;
                DropGrid.MaxWidth = Math.Min(e.NewSize.Width - 320, 450);
            }
        }
    }
}
