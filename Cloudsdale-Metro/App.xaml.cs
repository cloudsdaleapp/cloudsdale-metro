using System;
using Cloudsdale_Metro.Controllers;
using Cloudsdale_Metro.Views;
using Cloudsdale_Metro.Views.Controls;
using Cloudsdale_Metro.Views.LoadPages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;

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

        private static void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args) {
            var accountSettings = new SettingsCommand(
                "AccountSettings", "Account settings",
                command => new AccountSettings().FlyOut());

            if (Connection.SessionController.CurrentSession != null && !(Connection.MainFrame.Content is LoggingIn)) {
                args.Request.ApplicationCommands.Add(accountSettings);
            }
        }

        private static void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            CloudsdaleLib.ModelSettings.AppLastSuspended = DateTime.Now;
            deferral.Complete();
        }
    }
}
