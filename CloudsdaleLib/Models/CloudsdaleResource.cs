using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using CloudsdaleLib.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Windows.UI.Core;

namespace CloudsdaleLib.Models {
    public class CloudsdaleModel : INotifyPropertyChanged {

        public void CopyTo(CloudsdaleModel other) {
            var properties = GetType().GetRuntimeProperties();
            foreach (var property in properties) {
                var attribute = property.GetCustomAttribute<JsonPropertyAttribute>()
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
            Id = id;
        }

        [JsonProperty("id")]
        public readonly string Id;
    }
}
