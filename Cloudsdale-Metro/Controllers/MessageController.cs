using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Models;
using CloudsdaleLib.Providers;
using MetroFaye;
using Newtonsoft.Json.Linq;
using Windows.UI.Core;

namespace Cloudsdale_Metro.Controllers {
    public class MessageController : IMessageReciever, ICloudServicesProvider {
        private readonly Dictionary<string, CloudController> cloudControllers = new Dictionary<string, CloudController>();
        public CloudController CurrentCloud { get; set; }

        public async void OnMessage(JObject message) {
            await App.Connection.MainFrame.Dispatcher.RunAsync(
                CoreDispatcherPriority.Low, () => InternalOnMessage(message));
        }

        private void InternalOnMessage(JObject message) {
            var chanSplit = ((string)message["channel"]).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (chanSplit.Length < 2) return;

            switch (chanSplit[0]) {
                case "clouds":
                    if (cloudControllers.ContainsKey(chanSplit[1])) {
                        cloudControllers[chanSplit[1]].OnMessage(message);
                    }
                    break;
                case "users":
                    var sessionController = App.Connection.Session;
                    if (sessionController.CurrentSession == null) break;
                    if (chanSplit[1] == sessionController.CurrentSession.Id) {
                        App.Connection.Session.OnMessage(message);
                    }
                    break;
            }
        }

        public CloudController this[Cloud cloud] {
            get {
                if (!cloudControllers.ContainsKey(cloud.Id)) {
                    cloudControllers[cloud.Id] = new CloudController(cloud);
                }

                return cloudControllers[cloud.Id];
            }
        }

        public IStatusProvider StatusProvider(string cloudId) {
            return cloudControllers[cloudId];
        }

        public User GetBackedUser(string userId) {
            return App.Connection.UserController.GetUser(userId);
        }
    }
}
