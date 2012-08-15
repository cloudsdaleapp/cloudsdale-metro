using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Cloudsdale.Controllers.Data;
using Newtonsoft.Json;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace Cloudsdale.Models.Json {

    [JsonObject(MemberSerialization.OptIn)]
    public class ListUser : CloudsdaleItem {
        [JsonProperty("name")]
        public virtual string Name { get; set; }
        [JsonProperty("avatar")]
        public virtual Avatar Avatar { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ChatUser : ListUser, INotifyPropertyChanged {
        [JsonProperty("role")]
        public string Role;

        #region Role Format Properties
        public string RoleTag {
            get {
                switch (Role) {
                    case "creator":
                        return "founder";
                    case "moderator":
                        return "mod";
                    case "admin":
                    case "donor":
                        return Role;
                    default:
                        return "";
                }
            }
        }
        public Color RoleColor {
            get {
                switch (Role) {
                    case "creator":
                        return Color.FromArgb(0xFF, 0xFF, 0x1F, 0x1F);
                    case "admin":
                        return Color.FromArgb(0xFF, 0x1F, 0x7F, 0x1F);
                    case "moderator":
                        return Color.FromArgb(0xFF, 0xFF, 0xAF, 0x1F);
                    case "donor":
                        return Color.FromArgb(0xFF, 0x66, 0x00, 0xCC);
                }
                return default(Color);
            }
        }
        public Brush RoleBrush {
            get { return new SolidColorBrush(RoleColor); }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class User : ChatUser {
        [JsonProperty("time_zone")]
        public string TimeZone;
        [JsonProperty("member_since")]
        public DateTime? MemberSince;
        [JsonProperty("suspended_until")]
        public DateTime? SuspendedUntil;
        [JsonProperty("reason_for_suspension")]
        public string ReasonForSuspension;
        [JsonProperty("is_registered")]
        public bool? IsRegistered;
        [JsonProperty("is_transient")]
        public bool? IsTransient;
        [JsonProperty("is_banned")]
        public bool? IsBanned;
        [JsonProperty("is_member_of_a_cloud")]
        public bool? IsMemberOfACloud;
        [JsonProperty("has_an_avatar")]
        public bool? HasAnAvatar;
        [JsonProperty("has_read_tnc")]
        public bool? HasReadTnc;
        [JsonProperty("prosecutions")]
        public Prosecution[] Prosecutions;
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class LoggedInUser : User, INotifyPropertyChanged {
        [JsonProperty("auth_token")]
        public string AuthToken;
        [JsonProperty("email")]
        public string Email;
        [JsonProperty("needs_name_change")]
        public bool? NeedsNameChange;
        [JsonProperty("needs_password_change")]
        public bool? NeedsPasswordChange;
        [JsonProperty("needs_to_confirm_registration")]
        public bool? NeedsToConfirmRegistration;
        [JsonProperty("clouds")]
        public Cloud[] JsonClouds {
            get {
                var clouds = new Cloud[_clouds.Count];
                _clouds.CopyTo(clouds, 0);
                return clouds;
            }
            set {
                if (_clouds.Count < 1) {
                    var indices = new SortedDictionary<int, int>();
                    var unadded = new List<int>();
                    for (var x = 0; x < value.Length; ++x) {
                        var set = false;
                        for (var y = 0; y < ConnectionController.CloudOrder.Count; ++y) {
                            if (ConnectionController.CloudOrder[y] != value[x].Id) continue;
                            indices[y] = x;
                            set = true;
                            break;
                        }
                        if (!set) unadded.Add(x);
                    }
                    foreach (var index in indices) {
                        _clouds.Add(value[index.Value]);
                    }
                    foreach (var index in unadded) {
                        _clouds.Add(value[index]);
                    }
                } else {
                    foreach (var cloud in value) {
                        var set = false;
                        for (var i = 0; i < _clouds.Count; ++i) {
                            if (_clouds[i].Id == cloud.Id) {
                                _clouds[i] = cloud;
                                set = true;
                                break;
                            }
                        }
                        if (!set) _clouds.Add(cloud);
                    }
                }
            }
        }

        private readonly ObservableCollection<Cloud> _clouds = new ObservableCollection<Cloud>();
        public ObservableCollection<Cloud> Clouds {
            get { return _clouds; }
        }

        public void CloudsChanged() {
            if (Helpers.UIAccess) CloudsChangedInternal();
            else Helpers.RunInUI(CloudsChangedInternal, CoreDispatcherPriority.Normal);
        }

        private void CloudsChangedInternal() {
            OnPropertyChanged("Clouds");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
