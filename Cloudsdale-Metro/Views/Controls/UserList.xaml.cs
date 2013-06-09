using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cloudsdale_Metro.Controllers;
using WinRTXamlToolkit.AwaitableUI;
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
    public sealed partial class UserList {
        private readonly CloudController _controller;
        public UserList(CloudController controller) {
            InitializeComponent();
            DataContext = _controller = controller;
        }


        private async void UserList_OnLoaded(object sender, RoutedEventArgs e) {
            await this.WaitForNonZeroSizeAsync();
            DataContext = _controller;
        }
    }
}
