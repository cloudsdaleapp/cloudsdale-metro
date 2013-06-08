using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
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
            foreach (var property in properties) {
                var attribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute == null) continue;

                var value = property.GetValue(this);
                if (value != null) {
                    property.SetValue(other, value);
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

                var request = WebRequest.CreateHttp(attribute.Endpoint.Replace("[:id]", Id));
                request.Accept = "application/json";
                await ValidationRequest(request);

                string responseData;
                using (var response = await request.GetResponseAsync())
                using (var responseStream = response.GetResponseStream())
                using (var responseReader = new StreamReader(responseStream)) {
                    responseData = await responseReader.ReadToEndAsync();
                }

                var responseObject = JObject.Parse(responseData);
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

#pragma warning disable 1998
        protected virtual async Task ValidationRequest(HttpWebRequest request) {
            request.Method = "GET";
            if (Cloudsdale.SessionProvider.CurrentSession != null) {
                request.Headers["X-Auth-Token"] = Cloudsdale.SessionProvider.CurrentSession.AuthToken;
            }
        }
#pragma warning restore 1998

        [JsonIgnore]
        protected DateTime LastUpdated { get; set; }

        public override void CopyTo(CloudsdaleModel other) {
            base.CopyTo(other);
            LastUpdated = DateTime.Now;
        }

        public async Task UpdateProperty<T>(params KeyValuePair<string, JToken>[] properties) where T : CloudsdaleResource {
            var endpoint = GetType().GetTypeInfo().GetCustomAttribute<ResourceEndpointAttribute>();
            var model = new JObject();
            model[endpoint.RestModelType] = new JObject();
            model[endpoint.RestModelType]["id"] = Id;
            foreach (var property in properties) {
                model[endpoint.RestModelType][property.Key] = property.Value;
            }
            var requestData = Encoding.UTF8.GetBytes(model.ToString(Formatting.None));

            var request = WebRequest.CreateHttp(endpoint.UpdateEndpoint.Replace("[:id]", Id));
            request.Accept = "application/json";
            request.ContentType = "application/json";

            using (var requestStream = await request.GetRequestStreamAsync()) {
                await requestStream.WriteAsync(requestData, 0, requestData.Length);
                await requestStream.FlushAsync();
            }

            var response = await request.PerformRequest<T>();
            if (response.Flash != null) {
                await Cloudsdale.ModelErrorProvider.OnError(response);
                return;
            }

            response.Result.CopyTo(this);
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
