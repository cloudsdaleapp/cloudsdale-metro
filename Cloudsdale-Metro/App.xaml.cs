using System;
using Callisto.Controls;
using Cloudsdale_Metro.Controllers;
using Cloudsdale_Metro.Views;
using Cloudsdale_Metro.Views.Controls;
using Cloudsdale_Metro.Views.LoadPages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Cloudsdale_Metro {
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App {
        public readonly ConnectionController ConnectionController = new ConnectionController();

        public static ConnectionController Connection {
            get { return ((App)Current).ConnectionController; }
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            InitializeComponent();
            Suspending += OnSuspending;

            RequestedTheme = ApplicationTheme.Light;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args) {
            await ConnectionController.EnsureAppActivated();
            if (args.PreviousExecutionState != ApplicationExecutionState.Running &&
                args.PreviousExecutionState != ApplicationExecutionState.Suspended) {
                ConnectionController.Navigate(typeof(LoginPage));
            }

            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }

        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args) {
            var accountSettings = new SettingsCommand("AccountSettings", "Account settings", command => {
                var settings = new SettingsFlyout {
                    HeaderBrush = new SolidColorBrush(Color.FromArgb(0x7F, 0x1A, 0x91, 0xDB)),
                    HeaderText = "Account settings",
                    Background = new SolidColorBrush(Colors.Transparent),
                    Content = new AccountSettings()
                };

                var avatar = new BitmapImage(Connection.Session.CurrentSession.Avatar.Preview);
                settings.SmallLogoImageSource = avatar;

                settings.ContentBackgroundBrush = new SolidColorBrush(Color.FromArgb(0x7F, 0xF0, 0xF0, 0xF0));

                settings.IsOpen = true;
            });

            if (Connection.Session.CurrentSession != null && !(Connection.MainFrame.Content is LoggingIn)) {
                args.Request.ApplicationCommands.Add(accountSettings);
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            CloudsdaleLib.ModelSettings.AppLastSuspended = DateTime.Now;
            deferral.Complete();
        }
    }
}
