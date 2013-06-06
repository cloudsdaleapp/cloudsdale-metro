using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudsdale_Metro.Assets;
using Cloudsdale_Metro.Views;
using MetroFaye;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Cloudsdale_Metro.Controllers {
    public class ConnectionController {
        private Frame frame;
        private Frame connectView;

        public Frame MainFrame { get { return frame; } }

        public readonly SessionController Session = new SessionController();
        public MessageHandler Faye;

        public async Task EnsureAppActivated() {
            frame = Window.Current.Content as Frame;

            if (connectView == null) {
                connectView = new Frame {
                    Content = new Connecting(),
                    Transitions = new TransitionCollection {
                        new EdgeUIThemeTransition()
                    }
                };
            }

            await Session.LoadSession();

            if (frame == null) {
                frame = new Frame {
                    Transitions = new TransitionCollection {
                        new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Bottom }
                    }
                };
            }

            Window.Current.Content = connectView;

            Window.Current.Activate();

            await EnsureFayeConnection();
        }

        public void Navigate(Type pageType) {
            frame.Navigate(pageType);
        }

        public async Task EnsureFayeConnection() {
            if (Faye == null || !Faye.IsConnected) {
                frame.Content = connectView;

                Faye = MetroFaye.Faye.CreateClient(new Uri(Endpoints.PushAddress));

                var startTime = DateTime.Now;
                await Faye.ConnectAsync();
                var endTime = DateTime.Now;
                var pauseTime = 1.5 - (endTime - startTime).TotalSeconds;
                if (pauseTime > 0) {
                    await Task.Delay((int)(pauseTime * 1000));
                }
            }

            Window.Current.Content = frame;
        }
    }
}
