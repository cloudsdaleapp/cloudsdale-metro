using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudsdaleLib.Annotations;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class User : CloudsdaleResource {
        [JsonConstructor]
        public User(string id)
            : base(id) {
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }
        public string Role { get; set; }

        [JsonProperty("member_since")]
        public DateTime? MemberSince { get; set; }
    }
}
