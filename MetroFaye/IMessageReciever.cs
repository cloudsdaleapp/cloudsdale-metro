using Newtonsoft.Json.Linq;

namespace MetroFaye {
    public interface IMessageReciever {
        void OnMessage(JObject message);
    }
}
