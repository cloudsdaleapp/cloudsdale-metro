using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudsdaleLib.Annotations;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    [ResourceEndpoint("https://www.cloudsdale.org/v1/clouds/[:id]", RestModelType = "user")]
    public class User : CloudsdaleResource {
        [JsonConstructor]
        public User(string id) : base(id) {}

        #region Visual information
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("skype_name")]
        public string SkypeName { get; set; }
        [JsonProperty("also_known_as")]
        public string[] AlsoKnownAs { get; set; }
        #endregion

        #region Information relavent to internal treatment
        [JsonProperty("member_since")]
        public DateTime? MemberSince { get; set; }
        [JsonProperty("suspended_until")]
        public DateTime? SuspendedUntil { get; set; }
        [JsonProperty("reason_for_suspension")]
        public string SuspensionReason { get; set; }
        [JsonProperty("is_banned")]
        public bool? IsBanned { get; set; }
        [JsonProperty("is_registered")]
        public bool? IsRegistered { get; set; }
        [JsonProperty("has_read_tnc")]
        public bool? HasReadTnc { get; set; }
        [JsonProperty("is_member_of_a_cloud")]
        public bool? IsMemberOfACloud { get; set; }
        [JsonProperty("has_an_avatar")]
        public bool? HasAnAvatar { get; set; }
        #endregion

        public class CloudSpecificData {
        }
    }
}
