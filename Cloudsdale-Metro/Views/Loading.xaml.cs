using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudsdale.Controllers.Data;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloudsdale.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Loading {

        public const string PushUrl = "ws://push01.cloudsdale.org/push";

        public Loading() {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            JoinAnimation.Begin();
            var tryagain = true;
            while (tryagain) {
                bool fail = false;
                try {
                    var response = await ConnectionController.Connect();
                    if (response != null && (response.Successful ?? false)) {
                        foreach (var cloud in ConnectionController.CurrentUser.Clouds) {
                            ConnectionController.Subscribe(cloud);
                        }
                        await Navigate(typeof (Home));
                        tryagain = false;
                    } else {
                        fail = true;
                    }
                } catch {
                    fail = true;
                }
                if (!fail) continue;
                var dialog = new MessageDialog("There was an error connecting to cloudsdale.");
                dialog.Commands.Clear();
                dialog.Commands.Add(new UICommand("Retry"));
                dialog.Commands.Add(new UICommand("Quit"));
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;
                if ((await dialog.ShowAsync()).Label != "Retry") {
                    Application.Current.Exit();
                    return;
                }
            }
        }

        private async Task Navigate(Type t) {
            LeaveAnimation.Begin();
            await Task.Delay(200);
            Frame.Navigate(t);
        }
    }
}
