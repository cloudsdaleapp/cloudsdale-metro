using System;
using CloudsdaleLib.Annotations;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class Ban : CloudsdaleResource {
        private DateTime? _due;
        private DateTime? _issued;
        private bool? _active;
        private bool? _expired;
        private bool? _revoked;
        private string _reason;
        private string _jurisdictionId;
        private string _jurisdictionType;
        private string _enforcerId;
        private string _offenderId;
        private DateTime? _updated;
        public Ban(string id) : base(id) { }

        /// <summary>
        /// The date at which the ban will expire
        /// </summary>
        [JsonProperty("due")]
        public DateTime? Due {
            get { return _due; }
            set {
                if (value.Equals(_due)) return;
                _due = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The date at which the ban was originally issued
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? Issued {
            get { return _issued; }
            set {
                if (value.Equals(_issued)) return;
                _issued = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The date at which the ban was last updated
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime? Updated {
            get { return _updated; }
            set {
                if (value.Equals(_updated)) return;
                _updated = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The ID of the offending party
        /// </summary>
        [JsonProperty("offender_id")]
        public string OffenderId {
            get { return _offenderId; }
            set {
                if (value == _offenderId) return;
                _offenderId = value;
                OnPropertyChanged();
                OnPropertyChanged("Offender");
            }
        }

        /// <summary>
        /// The ID of the enforcer who issued the ban
        /// </summary>
        [JsonProperty("enforcer_id")]
        public string EnforcerId {
            get { return _enforcerId; }
            set {
                if (value == _enforcerId) return;
                _enforcerId = value;
                OnPropertyChanged();
                OnPropertyChanged("Enforcer");
            }
        }

        /// <summary>
        /// The type of jurisdiction where the ban was enacted
        /// </summary>
        [JsonProperty("jurisdiction_type")]
        public string JurisdictionType {
            get { return _jurisdictionType; }
            set {
                if (value == _jurisdictionType) return;
                _jurisdictionType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The ID of the jurisdiction in which the ban was enacted
        /// </summary>
        [JsonProperty("jurisdiction_id")]
        public string JurisdictionId {
            get { return _jurisdictionId; }
            set {
                if (value == _jurisdictionId) return;
                _jurisdictionId = value;
                OnPropertyChanged();
                OnPropertyChanged("Jurisdiction");
            }
        }

        /// <summary>
        /// The offending user
        /// </summary>
        public User Offender { get { return Cloudsdale.UserProvider.GetUser(OffenderId); } }
        /// <summary>
        /// The enforcing user
        /// </summary>
        public User Enforcer { get { return Cloudsdale.UserProvider.GetUser(EnforcerId); } }
        /// <summary>
        /// The cloud where the ban was enacted
        /// </summary>
        public Cloud Jurisdiction { get { return Cloudsdale.CloudProvider.GetCloud(JurisdictionId); } }

        /// <summary>
        /// The given reason for the offense
        /// </summary>
        [JsonProperty("reason")]
        public string Reason {
            get { return _reason; }
            set {
                if (value == _reason) return;
                _reason = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether the ban has been revoked
        /// </summary>
        [JsonProperty("revoke")]
        public bool? Revoked {
            get { return _revoked; }
            set {
                if (value.Equals(_revoked)) return;
                _revoked = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Whether the ban has expired
        /// </summary>
        [JsonProperty("has_expired")]
        public bool? Expired {
            get { return _expired; }
            set {
                if (value.Equals(_expired)) return;
                _expired = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// If the ban is still active
        /// </summary>
        [JsonProperty("is_active")]
        public bool? Active {
            get { return _active; }
            set {
                if (value.Equals(_active)) return;
                _active = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Bans cannot be validated
        /// </summary>
        /// <returns>Always returns false</returns>
        public override bool CanValidate() {
            return false;
        }

    }
}
