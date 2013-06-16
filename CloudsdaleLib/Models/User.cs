using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CloudsdaleLib.Helpers;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    [ResourceEndpoint(Endpoints.User, RestModelType = "user")]
    public class User : CloudsdaleResource, IAvatarUploadTarget {
        private string _name;
        private bool? _hasAnAvatar;
        private bool? _isMemberOfACloud;
        private bool? _hasReadTnc;
        private bool? _isRegistered;
        private bool? _isBanned;
        private string _suspensionReason;
        private DateTime? _suspendedUntil;
        private DateTime? _memberSince;
        private string[] _alsoKnownAs;
        private string _skypeName;
        private string _role;
        private Avatar _avatar;
        private string _username;

        [JsonConstructor]
        public User(string id) : base(id) {}

        #region Visual information
        [JsonProperty("name")]
        public string Name {
            get { return _name; }
            set {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("avatar")]
        public Avatar Avatar {
            get {
                if (_avatar != null) {
                    _avatar.Owner = this;
                }
                return _avatar;
            }
            set {
                if (Equals(value, _avatar)) return;
                _avatar = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("role")]
        public string Role {
            get { return _role; }
            set {
                if (value == _role) return;
                _role = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("skype_name")]
        public string SkypeName {
            get { return _skypeName; }
            set {
                if (value == _skypeName) return;
                _skypeName = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("also_known_as")]
        public string[] AlsoKnownAs {
            get { return _alsoKnownAs; }
            set {
                if (Equals(value, _alsoKnownAs)) return;
                _alsoKnownAs = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Information relavent to internal treatment
        [JsonProperty("member_since")]
        public DateTime? MemberSince {
            get { return _memberSince; }
            set {
                if (value.Equals(_memberSince)) return;
                _memberSince = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("suspended_until")]
        public DateTime? SuspendedUntil {
            get { return _suspendedUntil; }
            set {
                if (value.Equals(_suspendedUntil)) return;
                _suspendedUntil = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("reason_for_suspension")]
        public string SuspensionReason {
            get { return _suspensionReason; }
            set {
                if (value == _suspensionReason) return;
                _suspensionReason = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("is_banned")]
        public bool? IsBanned {
            get { return _isBanned; }
            set {
                if (value.Equals(_isBanned)) return;
                _isBanned = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("is_registered")]
        public bool? IsRegistered {
            get { return _isRegistered; }
            set {
                if (value.Equals(_isRegistered)) return;
                _isRegistered = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("has_read_tnc")]
        public bool? HasReadTnc {
            get { return _hasReadTnc; }
            set {
                if (value.Equals(_hasReadTnc)) return;
                _hasReadTnc = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("is_member_of_a_cloud")]
        public bool? IsMemberOfACloud {
            get { return _isMemberOfACloud; }
            set {
                if (value.Equals(_isMemberOfACloud)) return;
                _isMemberOfACloud = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("has_an_avatar")]
        public bool? HasAnAvatar {
            get { return _hasAnAvatar; }
            set {
                if (value.Equals(_hasAnAvatar)) return;
                _hasAnAvatar = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("username")]
        public string Username {
            get { return _username; }
            set {
                if (value == _username) return;
                _username = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("status")]
        public Status? Status;

        #endregion

        #region Avatar Upload

        public async Task UploadAvatar(Stream pictureStream, string mimeType) {
            HttpContent postData;
            using (var dataStream = new MemoryStream()) {
                using (pictureStream) {
                    await pictureStream.CopyToAsync(dataStream);
                }

                postData = new MultipartFormDataContent("--" + Guid.NewGuid() + "--") {
                    new ByteArrayContent(dataStream.ToArray()) {
                        Headers = {
                            ContentDisposition = new ContentDispositionHeaderValue("form-data") {
                                Name = "user[avatar]",
                                FileName = "GenericImage.png"
                            },
                            ContentLength = dataStream.Length,
                        }
                    }
                };
            }

            var request = new HttpClient {
                DefaultRequestHeaders = {
                    { "Accept", "application/json" },
                    { "X-Auth-Token", Cloudsdale.SessionProvider.CurrentSession.AuthToken }
                }
            };

            var response = await request.PostAsync(Endpoints.User.Replace("[:id]", Id), postData);
            var result = await JsonConvert.DeserializeObjectAsync<WebResponse<User>>(await response.Content.ReadAsStringAsync());

            if (response.StatusCode != HttpStatusCode.OK) {
                await Cloudsdale.ModelErrorProvider.OnError(result);
            } else {
                result.Result.CopyTo(this);
            }
        }

        #endregion
    }
}
