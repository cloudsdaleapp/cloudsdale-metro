using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Cloudsdale.Common;
using Cloudsdale.Controllers.Data;
using Cloudsdale.Models.Json;
using Windows.System;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloudsdale.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CloudView {

        public ScrollPage ChatScroller;

        public Cloud ThisCloud;

        public CloudView() {
            InitializeComponent();
            ChatScroller = new ScrollPage(ChatViewer, .5);

            SetCloud(ConnectionController.CurrentCloud);
        }

        public void SetCloud(Cloud cloud) {

            if (ThisCloud != null) {
                ThisCloud.Processor.MessageProcessor.Messages.CollectionChanged -= MessageReceived;
            }

            ThisCloud = cloud;
            DataContext = cloud;
            var controller = cloud.Processor;
            ChatItems.ItemsSource = controller.MessageProcessor.Messages;
            ThreadPool.RunAsync(async o => {
                await Task.Delay(100);
                Helpers.RunInUI(() => ChatScroller.Bottom(), CoreDispatcherPriority.Low);
            });
            ConnectionController.CurrentUser.CloudsChanged();

            ThisCloud.Processor.MessageProcessor.Messages.CollectionChanged += MessageReceived;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            ConnectionController.GetProcessor(ThisCloud).
                MessageProcessor.Messages.CollectionChanged -= MessageReceived;
            ChatScroller.Stop();
        }

        private void GoBack(object sender, RoutedEventArgs e) {
            Frame.GoBack();
        }

        void MessageReceived(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            ChatScroller.Bottom();
        }

        private void CloudClick(object sender, RoutedEventArgs e) {
            var button = (Button)sender;
            var cloud = (Cloud)button.DataContext;
            SetCloud(cloud);
        }

        private static readonly Random Rand = new Random();
        private void ChatGridLoaded(object sender, RoutedEventArgs e) {
            var grid = (Grid) sender;
            var anim = (Storyboard)grid.Resources["spinner"];
            anim.Children[0].Duration = new Duration(new TimeSpan(Rand.Next(5000000, 10000000)));
            //anim.Begin();
        }

        private bool _shiftpressed;
        private async void SendBoxKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e) {
            switch (e.Key) {
                case VirtualKey.Shift:
                    _shiftpressed = true;
                    break;
                case VirtualKey.Enter:
                    if (!_shiftpressed) {
                        e.Handled = true;
                        SendBox.IsEnabled = false;
                        var text = SendBox.Text;
                        SendBox.Text = "";
                        await ConnectionController.SendMessage(ThisCloud, text);
                        SendBox.IsEnabled = true;
                    } else {
                        SendBox.Text += "\n";
                    }
                    break;
            }
        }

        private void SendBoxKeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e) {
            if (e.Key == VirtualKey.Shift) _shiftpressed = false;
        }
    }
}
