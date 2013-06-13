using System.Linq;
using System.Threading.Tasks;
using Callisto.Controls;
using CloudsdaleLib.Models;
using Cloudsdale_Metro.Helpers;
using Newtonsoft.Json.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using WinRTXamlToolkit.AwaitableUI;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Cloudsdale_Metro.Views.Controls {
    public sealed partial class AccountSettings {
        private readonly Session session;

        public AccountSettings() {
            InitializeComponent();
            DataContext = session = App.Connection.Session.CurrentSession;
        }

        private async void LogoutClick(object sender, RoutedEventArgs e) {
            ((SettingsFlyout)Parent).IsOpen = false;
            await App.Connection.Session.LogOut();
        }

        private async void NameBox_OnLostFocus(object sender, RoutedEventArgs e) {
            var nameBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(nameBox.Text)) {
                session.Name = session.Name;
                return;
            }
            await DoUpdate("name", nameBox.Text, nameBox, NameModelError, NameProgress);
        }

        private async void SkypeBox_OnLostFocus(object sender, RoutedEventArgs e) {
            var skypeBox = (TextBox)sender;
            await DoUpdate("skype_name", skypeBox.Text, skypeBox, SkypeModelError, SkypeProgress);
        }

        private async void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            var statusBox = (ComboBox)sender;
            if (statusBox.SelectedIndex < 0 || statusBox.SelectedIndex > 3) return;
            var newStatus = (Status)statusBox.SelectedIndex;
            if (newStatus == session.PreferredStatus) return;

            await DoUpdate("preferred_status", newStatus.ToString(), statusBox, StatusModelError, StatusProgress);
        }

        private async Task DoUpdate(string property, JToken input, Control inputBox, TextBlock errorBlock, UIElement progress) {
            errorBlock.Text = "";
            progress.Visibility = Visibility.Visible;
            inputBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xA3, 0, 0, 0));

            try {
                var response = await session.UpdateProperty<Session>(true, property.KeyOf(input));
                if (response.Flash != null) {
                    var nameError = response.Errors.FirstOrDefault(error => error.Node == property);
                    SetError(inputBox, errorBlock, nameError != null ? nameError.Message : response.Flash.Message);
                }
            } catch {
                SetError(inputBox, errorBlock, "An error occured");
            }

            progress.Visibility = Visibility.Collapsed;
        }

        private void SetError(Control inputBox, TextBlock errorBlock, string message) {
            errorBlock.Text = message;
            inputBox.BorderBrush = new SolidColorBrush(Colors.Red);
        }

        private async void UIElement_OnKeyDown(object sender, KeyRoutedEventArgs e) {
            if (e.Key != VirtualKey.Enter) return;
            e.Handled = true;
            var box = (Control)sender;
            box.IsEnabled = false;
            await box.WaitForLayoutUpdateAsync();
            box.IsEnabled = true;
        }
    }
}
