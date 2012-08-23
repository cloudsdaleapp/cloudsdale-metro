using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudsdaleConnector.DataModels {
    [JsonObject]
    public class CloudsdaleItem : INotifyPropertyChanged {
        [JsonProperty("id")]
        public string Id;

        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
