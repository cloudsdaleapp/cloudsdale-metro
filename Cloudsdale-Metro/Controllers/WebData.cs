using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cloudsdale.Controllers.Data;
using Cloudsdale.Models.Web;

namespace Cloudsdale.Controllers {
    public static class WebData {
        public const string BaseUrl = "http://www.cloudsdale.org/v1/";
        public static readonly string[] Extensions = {
            "users/{id}.json",
            "clouds/{id}.json",
            "clouds/{id}/drops.json",
            "clouds/{id}/drops/search.json",
            "clouds/recent.json",
            "clouds/popular.json",
            "clouds/search.json",
            "clouds/{id}/chat/messages.json",
            "users/{id}/clouds.json"
        };

        public static async Task<WebJsonResponse<T>> GetDataAsync<T>(WebserviceItem category,
            string id = null, PostData post = null, params Kvp[] headers) {
            if (post == null) {
                return await Get<T>(category, id, headers);
            }
            return await Post<T>(category, id, post, headers);
        }

        private static async Task<WebJsonResponse<T>> Get<T>(WebserviceItem category,
            string id, IEnumerable<Kvp> headers) {
            try {
                var url = BaseUrl + Extensions[(int)category];
                if (!string.IsNullOrEmpty(id)) {
                    url = url.Replace("{id}", id);
                }
                var request = WebRequest.CreateHttp(url);
                foreach (var header in headers) {
                    request.Headers[header.Key] = header.Value;
                }
                var response = await Task<WebResponse>.Factory.FromAsync(
                    request.BeginGetResponse,
                    request.EndGetResponse, null);
                string responseString;
                using (var responseStream = response.GetResponseStream())
                using (var responseReader = new StreamReader(responseStream)) {
                    responseString = await responseReader.ReadToEndAsync();
                }
                var responseData = await Helpers.DeserializeAsync<WebJsonResponse<T>>(responseString);
                return responseData;
            } catch (Exception e) {
                return new WebJsonResponse<T> {
                    Data = default(T),
                    Errors = new[]{new WebJsonError {
                        Message = e.ToString(),
                        Type = e.GetType().ToString()
                    }}
                };
            }
        }

        private static async Task<WebJsonResponse<T>> Post<T>(WebserviceItem category,
            string id, PostData post, IEnumerable<Kvp> headers) {
            try {
                var url = BaseUrl + Extensions[(int)category];
                if (!string.IsNullOrEmpty(id)) {
                    url = url.Replace("{id}", id);
                }
                var request = WebRequest.CreateHttp(url);
                foreach (var header in headers) {
                    request.Headers[header.Key] = header.Value;
                }
                request.ContentType = post.Type;
                using (var requestStream = await Task<Stream>.Factory.FromAsync(
                           request.BeginGetRequestStream,
                           request.EndGetRequestStream,
                           null)) {
                    var data = Encoding.UTF8.GetBytes(post.Data);
                    await requestStream.WriteAsync(data, 0, data.Length);
                }
                var response = await Task<WebResponse>.Factory.FromAsync(
                    request.BeginGetResponse,
                    request.EndGetResponse, null);
                string responseString;
                using (var responseStream = response.GetResponseStream())
                using (var responseReader = new StreamReader(responseStream)) {
                    responseString = await responseReader.ReadToEndAsync();
                }
                if (typeof(T) == typeof(ConnectionController.SendMessageData)) Debugger.Break();
                var responseData = await Helpers.DeserializeAsync<WebJsonResponse<T>>(responseString);
                return responseData;
            } catch (Exception e) {
                return new WebJsonResponse<T> {
                    Data = default(T),
                    Errors = new[]{new WebJsonError {
                        Message = e.ToString(),
                        Type = e.GetType().ToString()
                    }}
                };
            }
        }
    }

    public enum WebserviceItem {
        /// <summary>
        /// Gets data about a user
        /// </summary>
        User = 0,
        /// <summary>
        /// Gets data about a cloud
        /// </summary>
        Cloud = 1,
        /// <summary>
        /// Gets the drops from a cloud. Use X-Result-Page in the headers to specify a page
        /// </summary>
        Drops = 2,
        /// <summary>
        /// 
        /// </summary>
        SearchDrops = 3,
        RecentClouds = 4,
        PopularClouds = 5,
        SearchClouds = 6,
        Messages = 7,
        UserClouds = 8,
    }

    public struct Kvp {
        public Kvp(string key, string value) {
            Key = key;
            Value = value;
        }

        public string Key;
        public string Value;
    }

    public class PostData {
        public string Data;
        public string Type;
    }
}
