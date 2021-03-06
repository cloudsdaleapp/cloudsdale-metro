﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CloudsdaleLib.Annotations;
using CloudsdaleLib.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Windows.UI.Core;

namespace CloudsdaleLib.Models {
    /// <summary>
    /// A model created and returned by the cloudsdale server, which may be updated
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CloudsdaleModel : INotifyPropertyChanged {
        public CloudsdaleModel() {
            UIMetadata = new UIMetadata(this);
        }

        /// <summary>
        /// Copies all the non-null properties of this model 
        /// marked with JsonProperty attributes to another model
        /// </summary>
        /// <param name="other"></param>
        public virtual void CopyTo(CloudsdaleModel other) {
            var properties = GetType().GetRuntimeProperties();
            var targetType = other.GetType();
            foreach (var property in properties) {
                var attribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                if (attribute == null) continue;

                var value = property.GetValue(this);
                if (value == null) continue;

                var targetProperty = targetType.GetRuntimeProperty(property.Name);
                targetProperty.SetValue(other, value);
            }
        }

        /// <summary>
        /// Metadata useful for UI display, provided by a MetadataProvider
        /// </summary>
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

        [OnError]
        public void OnError(StreamingContext context, ErrorContext errorContext) {
            errorContext.Handled = true;
            Debugger.Break();
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonErrorValueAttribute : Attribute {
        public JsonErrorValueAttribute(object defaultValue) {
            DefaultValue = defaultValue;
        }

        public object DefaultValue { get; set; }
    }

    /// <summary>
    /// A cloudsdale resource identified by a unique ID,
    /// which may be able to be updated directly
    /// from a cloudsdale endpoint
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CloudsdaleResource : CloudsdaleModel {
        [JsonConstructor]
        public CloudsdaleResource(string id) {
            LastUpdated = DateTime.Now;
            Id = id;
        }

        /// <summary>
        /// The unique ID of the resource
        /// </summary>
        [JsonProperty("id")]
        public readonly string Id;

        /// <summary>
        /// A boolean value determining if the resource's data could be invalid
        /// </summary>
        public bool Invalidated {
            get { return LastUpdated < ModelSettings.AppLastSuspended; }
        }

        /// <summary>
        /// Determines whether the model is able to
        /// be validated and updated at this time
        /// </summary>
        /// <returns>Whether the model can be upated</returns>
        public virtual bool CanValidate() {
            return true;
        }

        /// <summary>
        /// Asyncronously validates the model
        /// </summary>
        /// <returns>Whether the resource is now validated</returns>
        public Task<bool> Validate() {
            return Validate(false);
        }
        /// <summary>
        /// Asyncronously validates the model,
        /// ignoring whether it is considered invalid
        /// </summary>
        /// <returns>Whether validation succeded</returns>
        public Task<bool> ForceValidate() {
            return Validate(true);
        }
        /// <summary>
        /// Asyncronously validates the model
        /// </summary>
        /// <param name="force">Whether validation state is ignored</param>
        /// <returns>Validation success</returns>
        public async Task<bool> Validate(bool force) {
            if (!Invalidated && !force) return true;
            if (!CanValidate()) return false;

            var modelType = GetType().GetTypeInfo();
            var attribute = modelType.GetCustomAttribute<ResourceEndpointAttribute>();

            var requestUrl = attribute.Endpoint.Replace("[:id]", Id);
            var client = new HttpClient().AcceptsJson();
            var response = await ValidationRequest(client, requestUrl);

            var responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            var responseModel = (CloudsdaleModel)ObjectFromWebResult(responseObject).ToObject(GetType());
            responseModel.CopyTo(this);
            return true;
        }

        /// <summary>
        /// A virtual method which returns the token out of
        /// the path for the web response (See: session responses)
        /// </summary>
        /// <param name="response">The full web response</param>
        /// <returns>The expected return object</returns>
        protected virtual JToken ObjectFromWebResult(JToken response) {
            return response["result"];
        }

        /// <summary>
        /// Performs the http method, adding in neccesary headers
        /// and other models neccesary to fully retrieve an updated
        /// model
        /// </summary>
        /// <param name="client">The httpclient to perform the request on</param>
        /// <param name="uri">The uri to request from the server</param>
        /// <returns>The response object returned from the server</returns>
        protected virtual async Task<HttpResponseMessage> ValidationRequest(HttpClient client, string uri) {
            if (Cloudsdale.SessionProvider.CurrentSession != null) {
                client.DefaultRequestHeaders.Add("X-Auth-Token", Cloudsdale.SessionProvider.CurrentSession.AuthToken);
            }

            return await client.GetAsync(uri);
        }

        /// <summary>
        /// Helper method to object the type of rest model used to update the resource
        /// </summary>
        public string RestModelType {
            get { return GetType().GetTypeInfo().GetCustomAttribute<ResourceEndpointAttribute>().RestModelType; }
        }

        [JsonIgnore]
        protected DateTime LastUpdated { get; set; }

        /// <summary>
        /// Copies the properties from this resource to another
        /// </summary>
        /// <param name="other">The other resource</param>
        public override void CopyTo(CloudsdaleModel other) {
            base.CopyTo(other);
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Updates the current model on the server end
        /// </summary>
        /// <typeparam name="T">The derived type of model being updated</typeparam>
        /// <param name="cancelError">Whether to block sending errors to the standard error processor</param>
        /// <param name="properties">An array of Key-Value pairs containing the properties to update</param>
        /// <returns>A web response containing the result of the update</returns>
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

            if (responseMessage.StatusCode != HttpStatusCode.OK) {
                if (!cancelError)
                    await Cloudsdale.ModelErrorProvider.OnError(response);
                return response;
            }

            response.Result.CopyTo(this);
            return response;
        }
    }

    /// <summary>
    /// An attribute for derived types of CloudsdaleResources
    /// which provides information for how to validate and
    /// update the resources
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ResourceEndpointAttribute : Attribute {
        public ResourceEndpointAttribute(string endpoint) {
            Endpoint = endpoint;
            UpdateEndpoint = endpoint;
        }

        /// <summary>
        /// Endpoint to validate the model at, and used
        /// for updates if the UpdateEndpoint is not set
        /// </summary>
        public string Endpoint { get; private set; }
        /// <summary>
        /// Endpoint to use for updating the model
        /// </summary>
        public string UpdateEndpoint { get; set; }
        /// <summary>
        /// The REST wrapper property for updates to the model
        /// </summary>
        public string RestModelType { get; set; }
    }
}
