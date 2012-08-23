using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudsdaleConnector.DataModels {
    [JsonObject(MemberSerialization.OptIn)]
    public class User : CloudsdaleItem {

        private string _name;
        private string _role;
        private DateTime? _joinDate;

        [JsonProperty("name")]
        public string Name {
            get { return _name; } 
            set { 
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        [JsonProperty("role")]
        public string Role {
            get { return _role; }
            set {
                _role = value;
                OnPropertyChanged("Role");
            }
        }

        [JsonProperty("member_since")]
        public DateTime? JoinDate {
            get { return _joinDate; }
            set {
                _joinDate = value;
                OnPropertyChanged("JoinDate");
            }
        }
    }
}
