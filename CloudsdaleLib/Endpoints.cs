using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudsdaleLib {
    public static class Endpoints {
        public const string Base = "https://www.cloudsdale.org/";
        public const string ApiVersion = "v1";

        public const string SessionEndpoint = Base + ApiVersion + "/sessions";
        public const string UserEndpoint = Base + ApiVersion + "/users/[:id]";
        public const string CloudEndpoint = Base + ApiVersion + "/clouds/[:id]";
        public const string CloudMessagesEndpoint = CloudEndpoint + "/chat/messages";
        public const string CloudUsersEndpoint = CloudEndpoint + "/users";
        public const string CloudOnlineUsersEndpoint = CloudUsersEndpoint + "/online";

        public const string PushAddress = "wss://push.cloudsdale.org/push";
    }
}
