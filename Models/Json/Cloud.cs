using System;
using Newtonsoft.Json;
using Windows.UI.Xaml;

namespace Cloudsdale.Models.Json {
    [JsonObject(MemberSerialization.OptIn)]
    public class Cloud : CloudsdaleItem {
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

        public User FullOwner {
            get { return null; }
        }
    }
}
