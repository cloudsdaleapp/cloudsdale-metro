using System;
using System.Collections.Generic;
using Cloudsdale.Controllers.Data;
using Cloudsdale.Controllers.Login;
using Cloudsdale.Models.Json;
using System.Linq;
using System.Linq.Expressions;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloudsdale.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login {
        public Login() {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            if (await ConnectionController.LoadUserAsync()) {
                Frame.Navigate(typeof(Loading));
            } else {
                LayoutRoot.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e) {
            ContentPanel.Visibility = e.NewSize.Width < 1000 ? Visibility.Collapsed : Visibility.Visible;
            NarrowMessage.Visibility = e.NewSize.Width < 1000 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoginTextClick(object sender, PointerRoutedEventArgs e) {
            Window.Current.Activate();
        }

        private async void LoginClick(object sender, RoutedEventArgs e) {
            EmailBox.IsEnabled = false;
            PassBox.IsEnabled = false;
            LoginButton.IsEnabled = false;
            LoggingRing.IsActive = true;
            var d = new Dictionary<string, string>();
            d["email"] = EmailBox.Text;
            d["password"] = PassBox.Password;
            var loginRequest = new EmailLogin();
            var user = await loginRequest.Login(d);
            if (user == null) {
                LoggingRing.IsActive = false;
                switch (loginRequest.ErrorCode) {
                    default:
                        await new MessageDialog(
                            "An unknown error occured logging into cloudsdale.").ShowAsync();
                        break;
                    case 0:
                    case 1:
                        await new MessageDialog(
                            "There was an error connecting to cloudsdale.").ShowAsync();
                        break;
                    case 401:
                        await new MessageDialog(
                            "Incorrect username or password.").ShowAsync();
                        break;
                }

                EmailBox.IsEnabled = true;
                PassBox.IsEnabled = true;
                LoginButton.IsEnabled = true;
            } else {
                ConnectionController.CurrentUser = user;
                await ConnectionController.SaveUserAsync();
                Navigate(typeof(Loading));
            }
        }

        private void Navigate(Type t) {
            Frame.Navigate(t);
        }
    }
}
