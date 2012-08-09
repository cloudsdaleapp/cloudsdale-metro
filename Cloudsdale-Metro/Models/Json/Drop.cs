using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cloudsdale.Models.Json {
    [JsonObject(MemberSerialization.OptIn)]
    public class Drop : CloudsdaleItem {

        [JsonProperty("preview")]
        public Uri Preview { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
