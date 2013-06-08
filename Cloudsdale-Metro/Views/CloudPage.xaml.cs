using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CloudsdaleLib;
using CloudsdaleLib.Helpers;
using CloudsdaleLib.Models;
using Cloudsdale_Metro.Common;
using Cloudsdale_Metro.Controllers;
using Cloudsdale_Metro.Views.ChatConverters;
using Newtonsoft.Json;
using WinRTXamlToolkit.AwaitableUI;
using WinRTXamlToolkit.Controls.Extensions;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Cloudsdale_Metro.Views {
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CloudPage {
        private CloudController cloudController;

        public CloudPage() {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            CloudCanvas.StartLoop();
            cloudController = App.Connection.MessageController.CurrentCloud;
            await cloudController.EnsureLoaded();
            DefaultViewModel["Items"] = cloudController.Messages;
            cloudController.Messages.CollectionChanged += MessagesOnCollectionChanged;
            ScrollChat();
        }

        private void MessagesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs) {
            ScrollChat();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            CloudCanvas.Stop();
            cloudController.Messages.CollectionChanged -= MessagesOnCollectionChanged;
        }

        private void SendBoxKeyDown(object sender, KeyRoutedEventArgs e) {
            var sendBox = (TextBox)sender;
            if (e.Key != VirtualKey.Enter) return;
            var state = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
            var text = sendBox.Text;
            if ((state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down) {
                var index = sendBox.SelectionStart;
                var length = sendBox.SelectionLength;

                sendBox.Text = text.Substring(0, index) + "\n" + text.Substring(index + length);
                sendBox.SelectionLength = 0;
                sendBox.SelectionStart = index + 1;

                return;
            }
            sendBox.Text = string.Empty;
            SendMessage(text);
        }

        private async void SendMessage(string message) {
            var messageModel = new Message {
                Content = message.EscapeMessage(),
                Device = "desktop",
                ClientId = App.Connection.Faye.ClientId
            };

            var messageData = await JsonConvert.SerializeObjectAsync(messageModel);

            messageModel.Id = Guid.NewGuid().ToString();
            messageModel.Author = App.Connection.Session.CurrentSession;

            cloudController.Messages.AddToEnd(messageModel);

            var client = new HttpClient {
                DefaultRequestHeaders = {
                    { "Accept", "application/json" }, 
                    { "X-Auth-Token", App.Connection.Session.CurrentSession.AuthToken }
                }
            };
            var response = await client.PostAsync(Endpoints.CloudMessagesEndpoint.Replace("[:id]", cloudController.Cloud.Id),
                new StringContent(messageData) {
                    Headers = {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            );

            try {
                var responseText = await response.Content.ReadAsStringAsync();
                var fullMessage = await JsonConvert.DeserializeObjectAsync<WebResponse<Message>>(responseText);

                if (fullMessage == null) return;
                if (fullMessage.Flash != null) {
                    await App.Connection.ErrorController.OnError(fullMessage);
                    return;
                }

                fullMessage.Result.PreProcess();
                fullMessage.Result.CopyTo(messageModel);
            } catch (JsonException) { }
        }

        private async void ScrollChat() {
            await Task.Delay(100);
            await ChatList.WaitForNonZeroSizeAsync();
            await ChatScroll.ScrollToVerticalOffsetWithAnimation(ChatScroll.ScrollableHeight, 0.5);
        }

        private void ChatScroll_OnSizeChanged(object sender, SizeChangedEventArgs e) {
            ScrollChat();
        }
    }

    public class MessageTemplateSelector : DataTemplateSelector {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) {
            var message = (Message)item;
            var element = (FrameworkElement)container;
            if (Message.SlashMeFormat.IsMatch(message.Content)) {
                return (DataTemplate)element.GetFirstAncestorOfType<LayoutAwarePage>().Resources["StandardChatTemplate"];
            }
            return (DataTemplate)element.GetFirstAncestorOfType<LayoutAwarePage>().Resources["StandardChatTemplate"];
        }
    }
}
