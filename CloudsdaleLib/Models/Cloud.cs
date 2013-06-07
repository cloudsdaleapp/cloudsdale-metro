using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    [ResourceEndpoint(Endpoints.CloudEndpoint, RestModelType = "cloud")]
    public class Cloud : CloudsdaleResource {
        [JsonConstructor]
        public Cloud(string id) : base(id) { }
    }
}
