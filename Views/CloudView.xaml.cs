using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Cloudsdale.Common;
using Cloudsdale.Controllers.Data;
using Cloudsdale.Models.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
            ThisCloud = ConnectionController.CurrentCloud;
            DataContext = ThisCloud;
            InitializeComponent();
            var controller = ConnectionController.GetProcessor(
                ConnectionController.CurrentCloud.Id);
            ChatItems.ItemsSource = controller.MessageProcessor.Messages;
            ChatScroller = new ScrollPage(ChatViewer, .5);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            ConnectionController.GetProcessor(ThisCloud.Id).
                MessageProcessor.Messages.CollectionChanged += MessageReceived;
            ChatScroller.Bottom();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            ConnectionController.GetProcessor(ThisCloud.Id).
                MessageProcessor.Messages.CollectionChanged -= MessageReceived;
        }

        private void GoBack(object sender, RoutedEventArgs e) {
            Frame.GoBack();
        }

        void MessageReceived(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            ChatScroller.Bottom();
        }
    }
}
