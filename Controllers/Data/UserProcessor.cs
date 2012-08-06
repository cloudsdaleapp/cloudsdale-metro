using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;
using Windows.UI.Core;

namespace Cloudsdale.Controllers.Data {
    public class UserProcessor {
        #region Static
        private static readonly Dictionary<string, CensusUser> Users =
                            new Dictionary<string, CensusUser>();

        public static User RegisterData(CloudsdaleItem user) {
            if (user == null) return null;
            CensusUser cuser;
            if (Users.ContainsKey(user.Id)) {
                cuser = Users[user.Id];
            } else {
                cuser = new CensusUser(user.Id);
                Users[user.Id] = cuser;
            }

            #region ListUser
            if (user is ListUser) {
                var luser = user as ListUser;
                if (luser.Name != null && cuser.Name != luser.Name) {
                    cuser.Name = luser.Name;
                }
                if (cuser.Avatar == null) {
                    cuser.Avatar = luser.Avatar;
                } else if (luser.Avatar != null) {
                    if (luser.Avatar.Chat != null && cuser.Avatar.Chat != luser.Avatar.Chat) {
                        cuser.Avatar.Chat = luser.Avatar.Chat;
                    }
                    if (luser.Avatar.Normal != null && cuser.Avatar.Normal != luser.Avatar.Normal) {
                        cuser.Avatar.Normal = luser.Avatar.Normal;
                    }
                    if (luser.Avatar.Mini != null && cuser.Avatar.Mini != luser.Avatar.Mini) {
                        cuser.Avatar.Mini = luser.Avatar.Mini;
                    }
                    if (luser.Avatar.Preview != null && cuser.Avatar.Preview != luser.Avatar.Preview) {
                        cuser.Avatar.Preview = luser.Avatar.Preview;
                    }
                    if (luser.Avatar.Thumb != null && cuser.Avatar.Thumb != luser.Avatar.Thumb) {
                        cuser.Avatar.Thumb = luser.Avatar.Thumb;
                    }
                }
            }
            #endregion
            #region ChatUser
            if (user is ChatUser) {
                var chuser = user as ChatUser;
                if (chuser.Role != null && cuser.Role != chuser.Role) {
                    cuser.Role = chuser.Role;
                    cuser.OnPropertyChanged("RoleTag");
                    cuser.OnPropertyChanged("RoleBrush");
                }
            }
            #endregion
            #region Full User
            if (user is User) {
                var fuser = user as User;
                if (fuser.HasAnAvatar != null)
                    cuser.HasAnAvatar = fuser.HasAnAvatar;
                if (fuser.HasReadTnc != null)
                    cuser.HasReadTnc = fuser.HasReadTnc;
                if (fuser.IsBanned != null)
                    cuser.IsBanned = fuser.IsBanned;
                if (fuser.IsMemberOfACloud != null)
                    cuser.IsMemberOfACloud = fuser.IsMemberOfACloud;
                if (fuser.IsRegistered != null)
                    cuser.IsRegistered = fuser.IsRegistered;
                if (fuser.IsTransient != null)
                    cuser.IsTransient = fuser.IsTransient;
                if (fuser.MemberSince != null)
                    cuser.MemberSince = fuser.MemberSince;
                if (fuser.Prosecutions != null)
                    cuser.Prosecutions = fuser.Prosecutions;
                if (fuser.ReasonForSuspension != null)
                    cuser.ReasonForSuspension = fuser.ReasonForSuspension;
                if (fuser.SuspendedUntil != null)
                    cuser.SuspendedUntil = fuser.SuspendedUntil;
                if (fuser.TimeZone != null)
                    cuser.TimeZone = fuser.TimeZone;
            }
            #endregion

            return cuser;
        }
        #endregion

        #region Member
        public void Heartbeat(ListUser user) {
            
        }
        #endregion
    }

    public class CensusUser : User, INotifyPropertyChanged {
        public CensusUser(string id) {
            Id = id;
        }

        public override string Name {
            get {
                return base.Name;
            }
            set {
                base.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public override Avatar Avatar {
            get {
                return base.Avatar;
            }
            set {
                base.Avatar = value;
                OnPropertyChanged("Avatar");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected internal virtual void OnPropertyChanged(string propertyName) {
            if (!Helpers.UIAccess) {
                Helpers.RunInUI(() => OnPropertyChanged(propertyName), CoreDispatcherPriority.Normal);
            } else {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class CensusAvatar : Avatar, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public override Uri Chat {
            get {
                return base.Chat;
            }
            set {
                base.Chat = value;
                OnPropertyChanged("Chat");
            }
        }

        public override Uri Mini {
            get {
                return base.Mini;
            }
            set {
                base.Mini = value;
                OnPropertyChanged("Mini");
            }
        }

        public override Uri Normal {
            get {
                return base.Normal;
            }
            set {
                base.Normal = value;
                OnPropertyChanged("Normal");
            }
        }

        public override Uri Preview {
            get {
                return base.Preview;
            }
            set {
                base.Preview = value;
                OnPropertyChanged("Preview");
            }
        }

        public override Uri Thumb {
            get {
                return base.Thumb;
            }
            set {
                base.Thumb = value;
                OnPropertyChanged("Thumb");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName) {
            if (!Helpers.UIAccess) {
                Helpers.RunInUI(() => OnPropertyChanged(propertyName), CoreDispatcherPriority.Normal);
            } else {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
