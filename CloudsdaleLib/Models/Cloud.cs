using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CloudsdaleLib.Helpers;
using Newtonsoft.Json;

namespace CloudsdaleLib.Models {
    [JsonObject(MemberSerialization.OptIn)]
    [ResourceEndpoint(Endpoints.CloudEndpoint, RestModelType = "cloud")]
    public class Cloud : CloudsdaleResource, IAvatarUploadTarget {
        private string _name;
        private string[] _userIds;
        private string[] _moderatorIds;
        private string _ownerId;
        private Avatar _avatar;
        private bool? _hidden;
        private string _rules;
        private DateTime? _created;
        private string _description;

        [JsonConstructor]
        public Cloud(string id) : base(id) { }

        [JsonProperty("name")]
        public string Name {
            get { return _name; }
            set {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("description")]
        public string Description {
            get { return _description; }
            set {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("created_at")]
        public DateTime? Created {
            get { return _created; }
            set {
                if (value.Equals(_created)) return;
                _created = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("rules")]
        public string Rules {
            get { return _rules; }
            set {
                if (value == _rules) return;
                _rules = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("hidden")]
        public bool? Hidden {
            get { return _hidden; }
            set {
                if (value.Equals(_hidden)) return;
                _hidden = value;
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

        [JsonProperty("owner_id")]
        public string OwnerId {
            get { return _ownerId; }
            set {
                if (value == _ownerId) return;
                _ownerId = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("moderator_ids")]
        public string[] ModeratorIds {
            get { return _moderatorIds; }
            set {
                if (Equals(value, _moderatorIds)) return;
                _moderatorIds = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("user_ids")]
        public string[] UserIds {
            get { return _userIds; }
            set {
                if (Equals(value, _userIds)) return;
                _userIds = value;
                OnPropertyChanged();
            }
        }

        public async Task UploadAvatar(Stream pictureStream, string mimeType) {
            HttpContent postData;
            using (var dataStream = new MemoryStream()) {
                using (pictureStream) {
                    await pictureStream.CopyToAsync(dataStream);
                }

                postData = new MultipartFormDataContent("--" + Guid.NewGuid() + "--") {
                    new ByteArrayContent(dataStream.ToArray()) {
                        Headers = {
                            {
                                "Centent-Disposition",
                                new[] {
                                    "form-data", 
                                    "name=\"cloud[avatar]\"",
                                    "filename=\"GenericImage.png\""
                                }
                            },
                            { "Content-Type", dataStream.Length.ToString() }
                        }
                    }
                };
            }

            var request = new HttpClient {
                DefaultRequestHeaders = { { "Accept", "application/json" } }
            };

            var response = await request.PostAsync(Endpoints.UserEndpoint.Replace("[:id]", Id), postData);
            var result = await JsonConvert.DeserializeObjectAsync<WebResponse<Cloud>>(await response.Content.ReadAsStringAsync());

            if (response.StatusCode != HttpStatusCode.OK) {
                await Cloudsdale.ModelErrorProvider.OnError(result);
            } else {
                result.Result.CopyTo(this);
            }
        }
    }
}
