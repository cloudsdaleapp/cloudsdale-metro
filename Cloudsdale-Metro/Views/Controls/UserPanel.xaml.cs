using System.Net.Http;
using System.Net.Http.Headers;
using CloudsdaleLib;
using CloudsdaleLib.Helpers;
using CloudsdaleLib.Models;
using Cloudsdale_Metro.Common;
using System.Linq;
using Cloudsdale_Metro.Helpers;
using Newtonsoft.Json;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;

namespace Cloudsdale_Metro.Views.Controls {
    public sealed partial class UserPanel {
        private readonly LayoutAwarePage.ObservableDictionary<string, object> _defaultViewModel
            = new LayoutAwarePage.ObservableDictionary<string, object>();
        public IObservableMap<string, object> DefaultViewModel { get { return _defaultViewModel; } }

        public User User { get; set; }
        public Session Session { get; set; }
        public Cloud Cloud { get; set; }

        public UserPanel(User user) {
            User = user;
            Session = App.Connection.GetSession();
            Cloud = App.Connection.MessageController.CurrentCloud.Cloud;

            InitializeComponent();
        }

        private async void UserPanel_OnLoaded(object sender, RoutedEventArgs e) {
            DefaultViewModel["User"] = User;
            DefaultViewModel["HasAka"] = User.AlsoKnownAs.Length > 0;
            DefaultViewModel["IsModerator"] = Session.IsModerator();

            if (App.Connection.GetSession().IsModerator()) {
                DefaultViewModel["Bans"] = new Ban[0];
                DefaultViewModel["BansLoading"] = true;

                var client = new HttpClient {
                    DefaultRequestHeaders = {
                        {"Accept", "application/json"},
                        {"X-Auth-Token", Session.AuthToken}
                    },
                };

                var response = await client.GetAsync(
                    Endpoints.CloudUserBansEndpoint
                    .Replace("[:id]", Cloud.Id)
                    .Replace("[:offender_id]", User.Id));

                var resultData = await response.Content.ReadAsStringAsync();

                var bans = await JsonConvert.DeserializeObjectAsync<WebResponse<Ban[]>>(resultData);

                DefaultViewModel["BansLoading"] = false;
                DefaultViewModel["Bans"] = bans.Result;
            }

            DefaultViewModel["CanBan"] =
                User.Id != Session.Id
                && !User.IsModerator() || Session.IsOwner();
            DefaultViewModel["TrollBan"] =
                User.Role == "founder" && User.Role == "developer";
        }
    }
}
