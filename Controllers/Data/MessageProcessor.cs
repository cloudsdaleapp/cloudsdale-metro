using System.Collections.ObjectModel;
using Cloudsdale.Models.Json;
using Windows.UI.Core;

namespace Cloudsdale.Controllers.Data {
    public class MessageProcessor {

        private readonly ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages {
            get { return _messages; }
        }

        public void Add(Message message) {
            if (Helpers.UIAccess) InternalAdd(message);
            else Helpers.RunInUI(() => InternalAdd(message), CoreDispatcherPriority.Low);
        }

        private void InternalAdd(Message message) {
            var i = 0;
            while (i < _messages.Count && message.CompareTo(_messages[i]) < 1) ++i;

            if (i > 0 && _messages[i - 1].User.Id == message.User.Id) {
                _messages[i - 1].AddSubMessage(message);
            } else if (_messages[i].User.Id == message.User.Id) {
                _messages[i].AddSubMessage(message);
            } else {
                _messages.Insert(0, message);
                while (_messages.Count > 50) {
                    _messages.RemoveAt(0);
                }
            }
        }
    }
}
