using Newtonsoft.Json;

namespace Cloudsdale.Models.Web {
    [JsonObject(MemberSerialization.OptIn)]
    public class WebJsonResponse<T> {
        [JsonProperty("status")]
        public int? Status;

        [JsonProperty("errors")]
        public WebJsonError[] Errors;

        [JsonProperty("result")]
        public T Data;

        public static implicit operator T(WebJsonResponse<T> data) {
            return data.Data;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class WebJsonError {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("message")]
        public string Message;
    }
}
