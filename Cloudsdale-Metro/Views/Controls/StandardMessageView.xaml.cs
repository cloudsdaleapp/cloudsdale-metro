using System;
using CloudsdaleLib.Models;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cloudsdale_Metro.Views.Controls {
    public sealed partial class StandardMessageView {
        public StandardMessageView() {
            InitializeComponent();
        }

        private void StandardMessageView_OnSizeChanged(object sender, SizeChangedEventArgs e) {
            Separator.Width = e.NewSize.Width;

            if (e.NewSize.Width < 470) {
                DropGrid.Visibility = Visibility.Collapsed;
                AltDropGrid.Visibility = Visibility.Visible;
            } else {
                DropGrid.Visibility = Visibility.Visible;
                AltDropGrid.Visibility = Visibility.Collapsed;
                DropGrid.MaxWidth = Math.Min(e.NewSize.Width - 320, 450);
            }
        }

        private async void DropClicked(object sender, ItemClickEventArgs e) {
            var drop = (Drop)e.ClickedItem;
            await Launcher.LaunchUriAsync(drop.Url);
        }
    }
}
