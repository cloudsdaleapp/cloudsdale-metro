using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;
using Cloudsdale.Models.Web;
using Newtonsoft.Json;
using Windows.UI.Popups;

namespace Cloudsdale.Controllers.Login {
    public class EmailLogin : ILoginProcessor {

        public EmailLogin() {
            ErrorCode = -1;
        }

        public const string EmailLoginEndpoint = @"http://www.cloudsdale.org/v1/sessions";

        public async Task<LoggedInUser> Login(IDictionary<string, string> data) {
            await Task.Delay(0);
            var requestDataString = string.Format("email={0}&password={1}",
                Uri.EscapeDataString(data["email"]),
                Uri.EscapeDataString(data["password"]));
            var requestData = Encoding.UTF8.GetBytes(requestDataString);
            var request = WebRequest.CreateHttp(EmailLoginEndpoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json";
            try {
                using (var requestStream = await Task<Stream>.Factory.FromAsync(
                           request.BeginGetRequestStream,
                           request.EndGetRequestStream,
                           null)) {
                    await requestStream.WriteAsync(requestData, 0, requestData.Length);
                }
            } catch (WebException) {
                ErrorCode = 1;
                return null;
            }
            WebResponse response;
            try {
                response = await Task<WebResponse>.Factory.FromAsync(
                    request.BeginGetResponse,
                    request.EndGetResponse, null);
            } catch (WebException e) {
                try {
                    response = e.Response;
                    int status;
                    if (int.TryParse(response.Headers["Status"].Split(' ')[0], out status)) {
                        ErrorCode = status;
                    }
                } catch (NullReferenceException) {
                    ErrorCode = 0;
                }
                return null;
            }
            string responseString;
            using (var responseStream = response.GetResponseStream())
            using (var responseReader = new StreamReader(responseStream)) {
                responseString = await responseReader.ReadToEndAsync();
            }
            var responseData = await Helpers.DeserializeAsync<WebUserResponse>(responseString);
            return responseData.Data.User;
        }

        public int ErrorCode { get; private set; }
    }
}
