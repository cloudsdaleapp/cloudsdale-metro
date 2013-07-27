using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public sealed partial class AppSettings {
        public AppSettings() {
            InitializeComponent();
            InitializeFlyout();
        }

        public Models.AppSettings Settings {
            get { return Models.AppSettings.Settings; }
        }

        public override string Header {
            get { return "App Settings"; }
        }

        public override Uri Image {
            get { return null; }
        }
    }
}
