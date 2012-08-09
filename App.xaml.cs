using System;
using Cloudsdale.Controllers.Data;
using Cloudsdale.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Cloudsdale {
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            InitializeComponent();
            Suspending += OnSuspending;

            RequestedTheme = ApplicationTheme.Dark;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args) {
            Helpers.Dispatcher = Window.Current.Dispatcher;

            // Do not repeat app initialization when already running, just ensure that
            // the window is active

            if (args.PreviousExecutionState == ApplicationExecutionState.Running) {
                Window.Current.Activate();
                if (ConnectionController.Faye != null &&
                    !ConnectionController.Faye.Connected &&
                    ConnectionController.CurrentUser != null &&
                    Window.Current.Content is Frame) {
                    (Window.Current.Content as Frame).Navigate(typeof(Loading));
                }
                return;
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                //TODO: Load state from previously suspended application
            }

            // Create a Frame to act navigation context and navigate to the first page
            var rootFrame = new Frame();

            if (ConnectionController.Faye != null &&
                !ConnectionController.Faye.Connected &&
                ConnectionController.CurrentUser != null) {
                rootFrame.Navigate(typeof(Loading));
            } else if (!rootFrame.Navigate(typeof(Login))) {
                throw new Exception("Failed to create initial page");
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            await ConnectionController.SaveUserAsync();
            deferral.Complete();
        }
    }
}
