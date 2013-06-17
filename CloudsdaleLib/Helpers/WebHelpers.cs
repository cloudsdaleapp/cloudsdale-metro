using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudsdaleLib.Helpers {
    public class JsonContent : StringContent {
        public JsonContent(string json)
            : base(json) {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        public JsonContent(JToken json)
            : this(json.ToString(Formatting.None)) {
        }

        public JsonContent(object json)
            : this(JObject.FromObject(json).ToString(Formatting.None)) {
        }
    }

    public static class WebHelpers {
        public static HttpClient AcceptsJson(this HttpClient client) {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class WebResponse<T> {
        [JsonProperty("result")]
        public T Result;
        [JsonProperty("flash")]
        public FlashData Flash;

        private Error[] _errors;

        [JsonProperty("errors")]
        public Error[] Errors {
            get {
                if (_errors != null)
                    foreach (var error in _errors) {
                        error.Response = this;
                    }
                return _errors;
            }
            set { _errors = value; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class FlashData {
            [JsonProperty("title")]
            public string Title;
            [JsonProperty("message")]
            public string Message;
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class Error {
            internal WebResponse<T> Response;

            [JsonProperty("ref_node")]
            public string Node;
            [JsonProperty("message")]
            public string Message;

            public object NodeValue {
                get {
                    return (from property in typeof(T).GetTypeInfo().DeclaredProperties
                            let attribute = property.GetCustomAttribute<JsonPropertyAttribute>()
                            where attribute != null
                            where attribute.PropertyName == Node
                            select property.GetValue(Response.Result))
                            .FirstOrDefault();
                }
            }
        }
    }
}
