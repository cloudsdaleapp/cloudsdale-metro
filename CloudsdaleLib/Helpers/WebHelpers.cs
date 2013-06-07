using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudsdaleLib.Helpers {
    public static class WebHelpers {
        public static async Task<WebResponse<T>> PerformRequest<T>(this HttpWebRequest request) {
            return await JsonConvert.DeserializeObjectAsync<WebResponse<T>>(await request.ReadResponse());
        }

        public static async Task<string> ReadResponse(this HttpWebRequest request) {
            HttpWebResponse response;
            try {
                response = (HttpWebResponse)(await request.GetResponseAsync());
            } catch (WebException ex) {
                response = (HttpWebResponse)ex.Response;
            }
            return await response.InternalReadResponse();
        }

        private static async Task<string> InternalReadResponse(this HttpWebResponse response) {
            using (response)
            using (var responseStream = response.GetResponseStream())
            using (var responseReader = new StreamReader(responseStream)) {
                return await responseReader.ReadToEndAsync();
            }
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
                    var property = typeof(T).GetTypeInfo().GetDeclaredProperty(Node);
                    return property.GetValue(Response);
                }
            }
        }
    }
}
