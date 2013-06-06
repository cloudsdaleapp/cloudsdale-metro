using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroFaye;
using Newtonsoft.Json.Linq;

namespace Cloudsdale_Metro.Controllers {
    public class MessageController : IMessageReciever {
        public void OnMessage(JObject message) {
            
        }
    }
}
