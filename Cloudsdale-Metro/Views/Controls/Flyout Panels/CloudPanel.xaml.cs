using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CloudsdaleLib.Models;
using Cloudsdale_Metro.Assets;
using Cloudsdale_Metro.Common;
using Cloudsdale_Metro.Controllers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Cloudsdale_Metro.Views.Controls.Flyout_Panels {
    public sealed partial class CloudPanel {
        private readonly LayoutAwarePage.ObservableDictionary<string, object> _defaultViewModel
            = new LayoutAwarePage.ObservableDictionary<string, object>();
        public IObservableMap<string, object> DefaultViewModel { get { return _defaultViewModel; } }
        private readonly CloudController controller;

        public Cloud Cloud { get; set; }

        public CloudPanel(CloudController controller) {
            InitializeComponent();
            this.controller = controller;
            Cloud = this.controller.Cloud;
        }

        private void CloudPanel_OnLoaded(object sender, RoutedEventArgs e) {
            DefaultViewModel["Cloud"] = Cloud;
        }

        public override string Header {
            get { return controller.Cloud.Name; }
        }

        public override Uri Image {
            get { return null; }
        }
    }
}
