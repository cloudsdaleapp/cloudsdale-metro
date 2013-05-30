﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CloudsdaleLib.Annotations;
using CloudsdaleLib.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Windows.UI.Core;

namespace CloudsdaleLib.Models {
    public class CloudsdaleModel : INotifyPropertyChanged {

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

        public async Task<bool> Validate() {
            if (!Invalidated) return true;
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
                var responseModel = (CloudsdaleModel)responseObject.ToObject(GetType());
                responseModel.CopyTo(this);
            } catch (WebException exception) {
                OnValidationError(exception);
            }
            return true;
        }

#pragma warning disable 1998
        protected virtual async Task ValidationRequest(HttpWebRequest request) {
            request.Method = "GET";
            if (Cloudsdale.Instance.Session != null) {
                request.Headers["X-Auth-Token"] = Cloudsdale.Instance.Session.AuthToken;
            }
        }
#pragma warning restore 1998

        [JsonIgnore]
        public CloudsdaleController Controller { get; protected internal set; }

        [JsonIgnore]
        protected DateTime LastUpdated { get; set; }

        public override void CopyTo(CloudsdaleModel other) {
            base.CopyTo(other);
            LastUpdated = DateTime.Now;
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