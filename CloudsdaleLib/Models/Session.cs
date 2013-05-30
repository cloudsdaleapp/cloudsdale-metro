using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    public class Session : User {
        public Session(string id) : base(id) { }

        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }

        public override bool CanValidate() {
            return base.CanValidate() && Cloudsdale.Instance.Session != null;
        }
    }
}
