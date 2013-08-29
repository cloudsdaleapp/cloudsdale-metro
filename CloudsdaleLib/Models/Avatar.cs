using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Media;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    /// <summary>
    /// Avatar for a cloudsdale entity
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Avatar : CloudsdaleModel {
        private static readonly Regex MTimeRegex = new Regex(@"(?<=mtime=)([\d]+)");

        private Uri _normal;
        private Uri _mini;
        private Uri _thumb;
        private Uri _chat;
        private Uri _preview;
        
        internal CloudsdaleResource Owner { get; set; }

        /// <summary>
        /// 200x200 image avatar endpoint
        /// </summary>
        [JsonProperty("normal")]
        public Uri Normal {
            get { return _normal; }
            set {
                value = EnsureMTime(value);
                if (Equals(value, _normal)) return;
                _normal = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 24x24 image avatar endpoint
        /// </summary>
        [JsonProperty("mini")]
        public Uri Mini {
            get { return _mini; }
            set {
                value = EnsureMTime(value);
                if (Equals(value, _mini)) return;
                _mini = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 50x50 avatar endpoint
        /// </summary>
        [JsonProperty("thumb")]
        public Uri Thumb {
            get { return _thumb; }
            set {
                value = EnsureMTime(value);
                if (Equals(value, _thumb)) return;
                _thumb = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 40x40 image avatar endpoint
        /// </summary>
        [JsonProperty("chat")]
        public Uri Chat {
            get { return _chat; }
            set {
                value = EnsureMTime(value);
                if (Equals(value, _chat)) return;
                _chat = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 70x70 image avatar endpoint
        /// </summary>
        [JsonProperty("preview")]
        public Uri Preview {
            get { return _preview; }
            set {
                value = EnsureMTime(value);
                if (Equals(value, _preview)) return;
                _preview = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Avatar of specific image size
        /// </summary>
        /// <param name="size">Square dimmension for avatar retrieval</param>
        /// <returns>Endpoint for avatar of given size</returns>
        public Uri this[int size] {
            get {
                if (Owner == null) {
                    return Normal;
                }

                return new Uri(Endpoints.Avatar
                    .Replace("[:type]", Owner.RestModelType)
                    .Replace("[:id]", Owner.Id)
                    .Replace("[:size]", size.ToString())
                    .Replace("[:mtime]", Endpoints.ImageMTime.Ticks.ToString()));
            }
        }

        private static Uri EnsureMTime(Uri uri) {
            var uristr = uri.ToString();
            if (!MTimeRegex.IsMatch(uristr)) {
                uristr += (uristr.IndexOf('?') >= 0 ? '&' : '?') + "mtime=" + Endpoints.ImageMTime.Ticks;
                uri = new Uri(uristr);
            }
            return uri;
        }
    }
}
