using Newtonsoft.Json;

namespace CloudsdaleConnector.DataModels {
    [JsonObject(MemberSerialization.OptIn)]
    public struct WebResponse<T> {

        [JsonProperty("status")]
        public int Status;

        [JsonProperty("errors")]
        public CloudsdaleError[] Errors;

        [JsonProperty("result")]
        public T Result;
    }

    [JsonObject(MemberSerialization.OptIn)]
    public struct CloudsdaleError {
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("message")]
        public string Message;
    }
}
