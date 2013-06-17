using System;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class Avatar : CloudsdaleModel {
        private Uri _normal;
        private Uri _mini;
        private Uri _thumb;
        private Uri _chat;
        private Uri _preview;

        internal CloudsdaleResource Owner { get; set; }

        [JsonProperty("normal")]
        public Uri Normal {
            get { return _normal; }
            set {
                if (Equals(value, _normal)) return;
                _normal = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("mini")]
        public Uri Mini {
            get { return _mini; }
            set {
                if (Equals(value, _mini)) return;
                _mini = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("thumb")]
        public Uri Thumb {
            get { return _thumb; }
            set {
                if (Equals(value, _thumb)) return;
                _thumb = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("chat")]
        public Uri Chat {
            get { return _chat; }
            set {
                if (Equals(value, _chat)) return;
                _chat = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("preview")]
        public Uri Preview {
            get { return _preview; }
            set {
                if (Equals(value, _preview)) return;
                _preview = value;
                OnPropertyChanged();
            }
        }

        public Uri this[int size] {
            get {
                if (Owner == null) {
                    return Normal;
                }

                return new Uri(Endpoints.Avatar
                    .Replace("[:type]", Owner.RestModelType)
                    .Replace("[:id]", Owner.Id)
                    .Replace("[:size]", size.ToString()));
            }
        }
    }
}
