using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cloudsdale.Models.Json {
    [JsonObject(MemberSerialization.OptIn)]
    public class CloudsdaleItem {
        [JsonProperty("id")]
        public string Id;
    }
}
