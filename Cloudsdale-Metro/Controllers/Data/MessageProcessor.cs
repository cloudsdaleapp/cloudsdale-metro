using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;
using Windows.UI.Core;

namespace Cloudsdale.Controllers.Data {
    public class MessageProcessor {

        private readonly ObservableCollection<Message> _messages = new ObservableCollection<Message>();
        public ObservableCollection<Message> Messages {
            get { return _messages; }
        }

        public async Task Add(Message message) {
            foreach (var m in _messages) {
                if (m.Id == message.Id) return;
            }

            if (Helpers.UIAccess) InternalAdd(message);
            else await Helpers.RunInUI(() => InternalAdd(message), CoreDispatcherPriority.Low);
        }

        readonly Message.BaseComparer _comparer = new Message.BaseComparer();
        private void InternalAdd(Message message) {
            var i = 0;
            while (i < _messages.Count && _comparer.Compare(message, _messages[i]) >= 0) ++i;

            if (i > 0 && _messages[i - 1].User.Id == message.User.Id) {
                _messages[i - 1].AddSubMessage(message);
                _messages[i - 1] = _messages[i - 1];
            } else if (_messages.Count > 0 && i < _messages.Count && _messages[i].User.Id == message.User.Id) {
                _messages[i].AddSubMessage(message);
                _messages[i] = _messages[i];
            } else {
                _messages.Insert(i, message);
                while (_messages.Count > 50) {
                    _messages.RemoveAt(0);
                }
            }
        }
    }
}
