using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Cloudsdale_Metro.Views.Controls.View_Controls {
    public sealed partial class SkypeControl : UserControl {
        public SkypeControl() {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty SkypeNameProperty =
            DependencyProperty.Register("SkypeName", typeof(string), typeof(SkypeControl),
            new PropertyMetadata(default(string), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            var control = (SkypeControl)obj;
            var name = (string)args.NewValue;

            if (string.IsNullOrWhiteSpace(name)) {
                control.RootGrid.Visibility = Visibility.Collapsed;
                return;
            }

            control.DataContext = name;

            control.RootGrid.Visibility = Visibility.Visible;

            if (name.Contains(" ")) {
                control.SkypeButton.Visibility = Visibility.Collapsed;
                control.SkypeText.Visibility = Visibility.Visible;
            } else {
                control.SkypeButton.Visibility = Visibility.Visible;
                control.SkypeText.Visibility = Visibility.Collapsed;
            }
        }

        public string SkypeName {
            get { return (string)GetValue(SkypeNameProperty); }
            set { SetValue(SkypeNameProperty, value); }
        }

        private async void SkypeButton_OnClick(object sender, RoutedEventArgs e) {
            try {
                await Launcher.LaunchUriAsync(new Uri("skype:" + SkypeName + "?chat"));
            } catch (FormatException) {
            }
        }
    }
}
