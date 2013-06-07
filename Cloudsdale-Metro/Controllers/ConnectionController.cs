using System;
using System.Threading.Tasks;
using CloudsdaleLib;
using Cloudsdale_Metro.Models;
using Cloudsdale_Metro.Views;
using MetroFaye;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Endpoints = Cloudsdale_Metro.Assets.Endpoints;

namespace Cloudsdale_Metro.Controllers {
    public class ConnectionController {
        private Frame connectView;

        public Frame MainFrame { get; private set; }

        public readonly SessionController Session = new SessionController();
        public readonly ErrorController ErrorController = new ErrorController();
        public readonly MessageController MessageController = new MessageController();

        public ConnectionController() {
            Cloudsdale.SessionProvider = Session;
            Cloudsdale.ModelErrorProvider = ErrorController;
            Cloudsdale.CloudServicesProvider = MessageController;
            Cloudsdale.MetadataProviders["Selected"] = new BooleanMetadataProvider();
        }

        public MessageHandler Faye;

        public async Task EnsureAppActivated() {
            MainFrame = Window.Current.Content as Frame;

            if (connectView == null) {
                connectView = new Frame {
                    Content = new Connecting(),
                    Transitions = new TransitionCollection {
                        new EdgeUIThemeTransition()
                    }
                };
            }

            await Session.LoadSession();

            if (MainFrame == null) {
                MainFrame = new Frame {
                    Transitions = new TransitionCollection {
                        new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Right }
                    }
                };
            }

            Window.Current.Content = connectView;

            Window.Current.Activate();

            await EnsureFayeConnection();
        }

        public void Navigate(Type pageType) {
            MainFrame.Navigate(pageType);
        }

        public async Task EnsureFayeConnection() {
            if (Faye == null || !Faye.IsConnected) {
                MainFrame.Content = connectView;

                Faye = MetroFaye.Faye.CreateClient(new Uri(Endpoints.PushAddress));
                Faye.PrimaryReciever = MessageController;

                var error = false;
                try {
                    await Faye.ConnectAsync();
                } catch {
                    error = true;
                }
                if (error) {
                    var dialog = new MessageDialog("Couldn't connect to cloudsdale " +
                                                   "(are you connected to the internet?)",
                                                   "Connection Error");
                    await dialog.ShowAsync();
                    Window.Current.Close();
                }
            }

            Window.Current.Content = MainFrame;
        }
    }
}
