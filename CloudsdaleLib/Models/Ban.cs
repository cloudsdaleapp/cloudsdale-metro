using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    public class Ban : CloudsdaleResource {
        public Ban(string id) : base(id) { }

        [JsonProperty("due")]
        public DateTime? Due { get; set; }
        [JsonProperty("created_at")]
        public DateTime? Issued { get; set; }
        [JsonProperty("updated_at")]
        public DateTime? Updated { get; set; }

        [JsonProperty("offender_id")]
        public string OffenderId { get; set; }
        [JsonProperty("enforcer_id")]
        public string EnforcerId { get; set; }
        [JsonProperty("jurisdiction_type")]
        public string JurisdictionType { get; set; }
        [JsonProperty("jurisdiction_id")]
        public string JurisdictionId { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("revoke")]
        public bool? Revoked { get; set; }
        [JsonProperty("has_expired")]
        public bool? Expired { get; set; }
        [JsonProperty("is_active")]
        public bool? Active { get; set; }

        public override bool CanValidate() {
            return false;
        }
    }
}
