using Windows.UI.Xaml;

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
