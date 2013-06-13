using System;
using CloudsdaleLib.Models;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Cloudsdale_Metro.Views.Controls {
    public sealed partial class ActionMessageView {
        public ActionMessageView() {
            InitializeComponent();
        }

        private void ActionMessageView_OnSizeChanged(object sender, SizeChangedEventArgs e) {
            Separator.Width = e.NewSize.Width;
        }
    }
}
