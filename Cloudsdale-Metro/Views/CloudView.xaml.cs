using System.Threading.Tasks;
using Cloudsdale.Common;
using Cloudsdale.Controllers.Data;
using Cloudsdale.Models.Json;
using Windows.System;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloudsdale.Views {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CloudView {

        public ScrollPage ChatScroller;
        public ScrollPage ChatScroller2;

        public Cloud ThisCloud;

        public CloudView() {
            InitializeComponent();
            ChatScroller = new ScrollPage(ChatViewer, .2);
            ChatScroller2 = new ScrollPage(ChatViewer2, .2);

            LayoutRoot.MaxHeight = Window.Current.Bounds.Height;
            ChatViewer.Height = Window.Current.Bounds.Height - 80;

            SetCloud(ConnectionController.CurrentCloud);
        }

        public async Task SetCloud(Cloud cloud) {

            if (ThisCloud != null) {
                ThisCloud.Processor.MessageProcessor.Messages.CollectionChanged -= MessageReceived;
            }

            ThisCloud = cloud;
            ConnectionController.CurrentCloud = cloud;
            DataContext = cloud;
            var controller = cloud.Processor;

            if (!cloud.IsDataPreloaded) {
                ShowCover();
                ChatItems.ItemsSource = new Message[0];
                ChatItems2.ItemsSource = new Message[0];
                DropItems.ItemsSource = new Drop[0];
                UserItems.ItemsSource = new CensusUser[0];

                await cloud.PreloadData();

                HideCover();
            }

            ChatItems.ItemsSource = controller.MessageProcessor.Messages;
            ChatItems2.ItemsSource = controller.MessageProcessor.Messages;
            DropItems.ItemsSource = controller.DropProcessor.Drops;
            UserItems.ItemsSource = controller.UserProcessor.UserList;
            ThreadPool.RunAsync(async o => {
                await Task.Delay(100);
                Helpers.RunInUI(() => Bottom(false), CoreDispatcherPriority.Low);
            });
            ConnectionController.CurrentUser.CloudsChanged();

            ThisCloud.Processor.MessageProcessor.Messages.CollectionChanged += MessageReceived;
        }

        public void Bottom(bool animated = true) {
            if (animated) {
                lock (ChatScroller) {
                    ChatScroller.Bottom();
                    ChatScroller2.Bottom();
                }
            } else {
                ChatViewer.ScrollToVerticalOffset(double.PositiveInfinity);
                ChatViewer2.ScrollToVerticalOffset(double.PositiveInfinity);
            }
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
            ChatScroller2.Stop();
        }

        private void GoBack(object sender, RoutedEventArgs e) {
            Frame.GoBack();
        }

        void MessageReceived(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            Bottom();
        }

        private async void CloudClick(object sender, RoutedEventArgs e) {
            var button = (Button)sender;
            var cloud = (Cloud)button.DataContext;
            await SetCloud(cloud);
            TopAppBar.IsOpen = false;
        }

        private async void SendBoxKeyDown(object sender, KeyRoutedEventArgs e) {
            if (e.Key != VirtualKey.Enter) return;
            e.Handled = true;
            SendBox.IsEnabled = false;
            var text = SendBox.Text;
            SendBox.Text = "";
            await ConnectionController.SendMessage(ThisCloud, text);
            SendBox.IsEnabled = true;
            SendBox.Focus(FocusState.Programmatic);
        }

        void ShowCover() {
            coverBGB.Width = Window.Current.Bounds.Width;
            coverBGB.Height = Window.Current.Bounds.Height;
            coverBG.IsOpen = true;
        }
        void HideCover() {
            coverBG.IsOpen = false;
        }

        private void DropClick(object sender, RoutedEventArgs e) {
            var button = (Button)sender;
            var drop = (Drop)button.DataContext;

            Launcher.LaunchUriAsync(drop.Url);
        }

        private async void SendBox2KeyDown(object sender, KeyRoutedEventArgs e) {
            if (e.Key != VirtualKey.Enter) return;
            e.Handled = true;
            SendBox2.IsEnabled = false;
            var text = SendBox2.Text;
            SendBox2.Text = "";
            await ConnectionController.SendMessage(ThisCloud, text);
            SendBox2.IsEnabled = true;
            SendBox2.Focus(FocusState.Programmatic);
        }

        private void PageSizeChanged1(object sender, SizeChangedEventArgs e) {
            if (e.NewSize.Width < 700) {
                SnapView.Visibility = Visibility.Visible;
                RootScroller.Visibility = Visibility.Collapsed;
            } else {
                SnapView.Visibility = Visibility.Collapsed;
                RootScroller.Visibility = Visibility.Visible;
            }
            new Task(async () => {
                await Task.Delay(100);
                Helpers.RunInUI(() => Bottom(false), CoreDispatcherPriority.Low);
            }).Start();
            Bottom(false);
        }
    }
}
