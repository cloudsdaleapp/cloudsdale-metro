using System;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    [ResourceEndpoint(Endpoints.UserEndpoint, RestModelType = "user")]
    public class User : CloudsdaleResource {
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
            get { return _avatar; }
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

        #endregion

        [JsonObject(MemberSerialization.OptIn)]
        public class CloudContextInformation {
            
        }
    }
}
