using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CloudsdaleLib.Helpers;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    [ResourceEndpoint(Endpoints.SessionEndpoint, UpdateEndpoint = Endpoints.UserEndpoint, RestModelType = "user")]
    public class Session : User {
        private string _authToken;
        private string _email;
        private Status _preferredStatus;
        private bool? _needsToConfirmRegistration;
        private bool? _needsPasswordChange;
        private bool? _needsNameChange;
        private bool? _needsEmailChange;
        private List<Cloud> _clouds;
        private List<Ban> _bans;

        [JsonConstructor]
        public Session(string id) : base(id) { }

        [JsonProperty("auth_token")]
        public string AuthToken {
            get { return _authToken; }
            set {
                if (value == _authToken) return;
                _authToken = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("email")]
        public string Email {
            get { return _email; }
            set {
                if (value == _email) return;
                _email = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("preferred_status")]
        public Status PreferredStatus {
            get { return _preferredStatus; }
            set {
                if (value == _preferredStatus) return;
                _preferredStatus = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("needs_to_confirm_registration")]
        public bool? NeedsToConfirmRegistration {
            get { return _needsToConfirmRegistration; }
            set {
                if (value.Equals(_needsToConfirmRegistration)) return;
                _needsToConfirmRegistration = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("needs_password_change")]
        public bool? NeedsPasswordChange {
            get { return _needsPasswordChange; }
            set {
                if (value.Equals(_needsPasswordChange)) return;
                _needsPasswordChange = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("needs_name_change")]
        public bool? NeedsNameChange {
            get { return _needsNameChange; }
            set {
                if (value.Equals(_needsNameChange)) return;
                _needsNameChange = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("NeedsEmailChange")]
        public bool? NeedsEmailChange {
            get { return _needsEmailChange; }
            set {
                if (value.Equals(_needsEmailChange)) return;
                _needsEmailChange = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("clouds")]
        public List<Cloud> Clouds {
            get { return _clouds; }
            set {
                if (Equals(value, _clouds)) return;
                if (value != null)
                    for (var i = 0; i < value.Count; ++i) {
                        value[i] = Cloudsdale.CloudProvider.UpdateCloud(value[i]);
                    }
                _clouds = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("bans")]
        public List<Ban> Bans {
            get { return _bans; }
            set {
                if (Equals(value, _bans)) return;
                _bans = value;
                OnPropertyChanged();
            }
        }

        protected override Newtonsoft.Json.Linq.JToken ObjectFromWebResult(Newtonsoft.Json.Linq.JToken response) {
            return base.ObjectFromWebResult(response)["user"];
        }

        protected override async Task<HttpResponseMessage> ValidationRequest(HttpClient client, string requestUrl) {
            var requestModel = await JsonConvert.SerializeObjectAsync(new {
                oauth = new {
                    token = BCrypt.Net.BCrypt.HashPassword(Id + "cloudsdale", Endpoints.InternalToken),
                    client_type = "WinRT",
                    provider = "cloudsdale",
                    uid = Id,
                }
            }, Formatting.None);

            return await client.PostAsync(requestUrl, new JsonContent(requestModel));
        }
    }
}
