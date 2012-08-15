using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudsdale.Controllers.Data;
using Windows.UI.Core;
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
            if (await ConnectionController.HandledUIConnect()) {
                ConnectionController.LostConnection += () => Helpers.RunInUI(async () => {
                    await ConnectionController.HandledUIConnect();
                }, CoreDispatcherPriority.Normal);
                await Navigate(typeof(Home));
            }
        }

        private async Task Navigate(Type t) {
            LeaveAnimation.Begin();
            await Task.Delay(200);
            Frame.Navigate(t);
        }
    }
}
