using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;
using Newtonsoft.Json;

namespace Cloudsdale.Models.Web {
    [JsonObject(MemberSerialization.OptIn)]
    public class WebUserResponse : WebJsonResponse<WebUserResponse.UserResponse> {
        [JsonObject(MemberSerialization.OptIn)]
        public class UserResponse {
            [JsonProperty("user")]
            public LoggedInUser User;
        }
    }
}
