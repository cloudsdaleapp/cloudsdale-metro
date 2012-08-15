using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Cloudsdale.Controllers;
using Cloudsdale.Controllers.Data;
using Newtonsoft.Json;

namespace Cloudsdale.Models.Json {
    [JsonObject(MemberSerialization.OptIn)]
    public class Cloud : CloudsdaleItem, INotifyPropertyChanged {
        public Cloud() {
            IsDataPreloaded = false;
        }

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

        public bool IsDataPreloaded { get; private set; }

        private bool _inloading;
        public async Task PreloadData() {
            if (_inloading) return;
            _inloading = true;
            var messages = await WebData.GetDataAsync<Message[]>(WebserviceItem.Messages, Id);
            foreach (var message in messages.Data) {
                Processor.MessageProcessor.Add(message);
            }
            var drops = await WebData.GetDataAsync<Drop[]>(WebserviceItem.Drops, Id);
            await ConnectionController.Faye.Subscribe("/clouds/{id}/users".Replace("{id}", Id));
            Processor.DropProcessor.Backload(drops.Data);
            IsDataPreloaded = true;
            _inloading = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
