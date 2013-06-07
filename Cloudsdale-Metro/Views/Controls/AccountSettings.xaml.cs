using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Callisto.Controls;
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

namespace Cloudsdale_Metro.Views.Controls {
    public sealed partial class AccountSettings {
        public AccountSettings() {
            InitializeComponent();
        }

        private async void LogoutClick(object sender, RoutedEventArgs e) {
            ((SettingsFlyout)Parent).IsOpen = false;
            await App.Connection.Session.LogOut();
        }
    }
}
