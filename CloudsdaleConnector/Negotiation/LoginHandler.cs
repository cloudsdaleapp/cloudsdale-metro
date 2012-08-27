using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CloudsdaleConnector.Negotiation {
    public abstract class LoginHandler {
        private const string LoginUri = "http://www.cloudsdale.org/v1/sessions";

        public abstract Task<JObject> Login();

        protected async Task<JObject> SendLogin(string postData) {
            var request = WebRequest.CreateHttp(LoginUri);
            var data = Encoding.UTF8.GetBytes(postData);
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            using (var requestStream = await request.GetRequestStreamAsync()) {
                await requestStream.WriteAsync(data, 0, data.Length);
                await requestStream.FlushAsync();
            }
            using (var response = await request.GetResponseAsync())
            using (var responseStream = response.GetResponseStream())
            using (var responseReader = new StreamReader(responseStream)) {
                return JObject.Parse(await responseReader.ReadToEndAsync());
            }
        }
    }
}
