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
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class WebJsonError {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("message")]
        public string Message;
    }
}
