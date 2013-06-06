using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudsdale_Metro.Assets;
using Cloudsdale_Metro.Views;
using MetroFaye;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cloudsdale_Metro.Controllers {
    public class ConnectionController {
        private Frame frame;
        private object alternateContent;
        private Connecting connectView;

        public readonly SessionController Session = new SessionController();
        public MessageHandler Faye;

        public async Task EnsureAppActivated() {
            frame = Window.Current.Content as Frame;

            if (connectView == null) {
                connectView = new Connecting();
            }

            await Session.LoadSession();

            if (frame == null) {
                frame = new Frame { Content = connectView };

                Window.Current.Content = frame;
            }

            Window.Current.Activate();

            await EnsureFayeConnection();
        }

        public void Navigate(Type pageType) {
            Navigate(Activator.CreateInstance(pageType));
        }

        public async void Navigate(object page) {
            alternateContent = page;
            await EnsureFayeConnection();
        }

        public async Task EnsureFayeConnection() {
            if (Faye == null || !Faye.IsConnected) {
                frame.Content = connectView;

                Faye = MetroFaye.Faye.CreateClient(new Uri(Endpoints.PushAddress));

                await Faye.ConnectAsync();
            }

            if (alternateContent != null) {
                frame.Content = alternateContent;
            }
        }
    }
}
