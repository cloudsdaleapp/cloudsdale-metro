using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MetroFaye {
    public interface IMessageReciever {
        void OnMessage(JObject message);
    }
}
