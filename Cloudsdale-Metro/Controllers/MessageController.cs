using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Providers;
using MetroFaye;
using Newtonsoft.Json.Linq;

namespace Cloudsdale_Metro.Controllers {
    public class MessageController : IMessageReciever, ICloudServicesProvider {
        private readonly Dictionary<string, CloudController> cloudControllers = new Dictionary<string, CloudController>();

        public void OnMessage(JObject message) {

        }

        public IStatusProvider StatusProvider(string cloudId) {
            return cloudControllers[cloudId];
        }
    }
}
