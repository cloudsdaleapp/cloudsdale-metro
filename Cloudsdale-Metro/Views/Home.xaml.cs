using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Cloudsdale.Controllers.Data;
using Cloudsdale.Models.Json;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloudsdale.Views {
    /// <summary> 
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home {
        public Home() {
            InitializeComponent();
            DataContext = ConnectionController.CurrentUser;
            CloudView.ItemsSource = ConnectionController.CurrentUser.Clouds;
            CloudView.Height = Window.Current.Bounds.Height - 100;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }

        private async Task Navigate(Type t) {
            await Task.Delay(0);
            Frame.Navigate(t);
        }

        private async void LogoutClick(object sender, RoutedEventArgs e) {
            ConnectionController.Faye.Socket.Close(1000, "");
            ConnectionController.Faye = null;
            await Helpers.DeleteDataFileAsync("CurrentUser.gzip");
            await Navigate(typeof(Login));
        }

        private void CloudViewRightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e) {
            ((StackPanel)sender).Background = new SolidColorBrush(Colors.Green);
            BottomAppBar = new AppBar {
                Content = new ContentControl { ContentTemplate = CloudItemAppBarContent },
                DataContext = ((FrameworkElement) sender).DataContext,
            };
            BottomAppBar.Closed += (o, o1) => {
                ((StackPanel) sender).Background = new SolidColorBrush(Colors.CornflowerBlue);
            };
        }

        private void CloudViewItemClick(object sender, ItemClickEventArgs e) {
            if (e.ClickedItem is Cloud) {
                ConnectionController.CurrentCloud = e.ClickedItem as Cloud;
                Navigate(typeof (CloudView));
            }
        }
    }
}
