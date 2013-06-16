using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Annotations;
using CloudsdaleLib.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.UI.Core;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class CloudsdaleModel : INotifyPropertyChanged {
        public CloudsdaleModel() {
            UIMetadata = new UIMetadata(this);
        }

        public virtual void CopyTo(CloudsdaleModel other) {
            var properties = GetType().GetRuntimeProperties();
            var targetType = other.GetType();
            foreach (var property in properties) {
                var attribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute == null) continue;

                var value = property.GetValue(this);
                if (value != null) {
                    var targetProperty = targetType.GetRuntimeProperty(property.Name);
                    targetProperty.SetValue(other, value);
                }
            }
        }

        public UIMetadata UIMetadata { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected internal virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler == null) return;
            if (ModelSettings.Dispatcher != null && !ModelSettings.Dispatcher.HasThreadAccess) {
                ModelSettings.Dispatcher.RunAsync(CoreDispatcherPriority.Low,
                                                  () => handler(this, new PropertyChangedEventArgs(propertyName)));
            } else {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class CloudsdaleResource : CloudsdaleModel {
        [JsonConstructor]
        public CloudsdaleResource(string id) {
            LastUpdated = DateTime.Now;
            Id = id;
        }

        [JsonProperty("id")]
        public readonly string Id;

        public bool Invalidated {
            get { return LastUpdated < ModelSettings.AppLastSuspended; }
        }

        public virtual bool CanValidate() {
            return true;
        }

        protected virtual void OnValidationError(WebException exception) { }

        public Task<bool> Validate() {
            return Validate(false);
        }
        public Task<bool> ForceValidate() {
            return Validate(true);
        }
        public async Task<bool> Validate(bool force) {
            if (!Invalidated && !force) return true;
            if (!CanValidate()) return false;

            var modelType = GetType().GetTypeInfo();
            var attribute = modelType.GetCustomAttribute<ResourceEndpointAttribute>();

            try {

                var requestUrl = attribute.Endpoint.Replace("[:id]", Id);
                var client = new HttpClient {
                    DefaultRequestHeaders = {
                        {"Accept", "application/json"}
                    }
                };
                var response = await ValidationRequest(client, requestUrl);

                var responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                var responseModel = (CloudsdaleModel)ObjectFromWebResult(responseObject).ToObject(GetType());
                responseModel.CopyTo(this);
            } catch (WebException exception) {
                OnValidationError(exception);
            }
            return true;
        }

        protected virtual JToken ObjectFromWebResult(JToken response) {
            return response["result"];
        }

        protected virtual async Task<HttpResponseMessage> ValidationRequest(HttpClient client, string uri) {
            if (Cloudsdale.SessionProvider.CurrentSession != null) {
                client.DefaultRequestHeaders.Add("X-Auth-Token", Cloudsdale.SessionProvider.CurrentSession.AuthToken);
            }

            return await client.GetAsync(uri);
        }

        public string RestModelType {
            get { return GetType().GetTypeInfo().GetCustomAttribute<ResourceEndpointAttribute>().RestModelType; }
        }

        [JsonIgnore]
        protected DateTime LastUpdated { get; set; }

        public override void CopyTo(CloudsdaleModel other) {
            base.CopyTo(other);
            LastUpdated = DateTime.Now;
        }

        public async Task<WebResponse<T>> UpdateProperty<T>(
            bool cancelError,
            params KeyValuePair<string, JToken>[] properties)
            where T : CloudsdaleResource {

            var endpoint = GetType().GetTypeInfo().GetCustomAttribute<ResourceEndpointAttribute>();
            var model = new JObject();
            model[endpoint.RestModelType] = new JObject();
            model[endpoint.RestModelType]["id"] = Id;
            foreach (var property in properties) {
                model[endpoint.RestModelType][property.Key] = property.Value;
            }

            var requestUrl = endpoint.UpdateEndpoint.Replace("[:id]", Id);
            var client = new HttpClient {
                DefaultRequestHeaders = {
                    {"Accept", "application/json"},
                    {"X-Auth-Token", Cloudsdale.SessionProvider.CurrentSession.AuthToken}
                }
            };

            var responseMessage = await client.PutAsync(requestUrl, new JsonContent(model));
            var responseData = await responseMessage.Content.ReadAsStringAsync();
            var response = await JsonConvert.DeserializeObjectAsync<WebResponse<T>>(responseData);

            if (response.Flash != null) {
                if (!cancelError)
                    await Cloudsdale.ModelErrorProvider.OnError(response);
                return response;
            }

            response.Result.CopyTo(this);
            return response;
        }
    }

    public class ResourceEndpointAttribute : Attribute {
        public ResourceEndpointAttribute(string endpoint) {
            Endpoint = endpoint;
            UpdateEndpoint = endpoint;
        }

        public string Endpoint { get; set; }
        public string UpdateEndpoint { get; set; }
        public string RestModelType { get; set; }
    }
}
