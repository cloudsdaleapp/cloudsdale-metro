using System;
using System.ComponentModel;
using Cloudsdale.Controllers.Data;
using Newtonsoft.Json;
using Windows.UI.Xaml;

namespace Cloudsdale.Models.Json {
    [JsonObject(MemberSerialization.OptIn)]
    public class Cloud : CloudsdaleItem, INotifyPropertyChanged {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("rules")]
        public string Rules { get; set; }
        [JsonProperty("created_at")]
        public DateTime? Created { get; set; }
        [JsonProperty("hidden")]
        public bool? Hidden { get; set; }
        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }
        [JsonProperty("is_transient")]
        public bool? IsTransient { get; set; }
        [JsonProperty("owner")]
        public CloudsdaleItem Owner;
        [JsonProperty("moderators")]
        public CloudsdaleItem[] Moderators;
        [JsonProperty("chat")]
        public CloudChatInfo Chat { get; set; }

        [JsonObject(MemberSerialization.OptIn)]
        public class CloudChatInfo {
            [JsonProperty("last_message_at")]
            public DateTime? LastMessageAt;
        }

        public void UpdateIsCurrent() {
            OnPropertyChanged("IsNotCurrentCloud");
        }

        public User FullOwner {
            get { return null; }
        }

        public bool IsNotCurrentCloud {
            get { return ConnectionController.CurrentCloud.Id != Id; }
        }

        public CloudProcessor Processor {
            get { return ConnectionController.GetProcessor(this); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
