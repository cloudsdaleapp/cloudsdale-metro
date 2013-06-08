using System;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class Drop : CloudsdaleModel  {
        public readonly string Id;
        private string _title;
        private Uri _url;
        private Uri _preview;

        public Drop(string id) {
            Id = id;
        }

        public string Title {
            get { return _title; }
            set {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public Uri Url {
            get { return _url; }
            set {
                if (Equals(value, _url)) return;
                _url = value;
                OnPropertyChanged();
            }
        }

        public Uri Preview {
            get { return _preview; }
            set {
                if (Equals(value, _preview)) return;
                _preview = value;
                OnPropertyChanged();
            }
        }
    }
}
