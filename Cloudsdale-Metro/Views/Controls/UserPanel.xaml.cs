using CloudsdaleLib.Models;
using Cloudsdale_Metro.Common;
using System.Linq;
using Cloudsdale_Metro.Helpers;
using Windows.Foundation.Collections;

namespace Cloudsdale_Metro.Views.Controls {
    public sealed partial class UserPanel {
        private readonly LayoutAwarePage.ObservableDictionary<string, object> _defaultViewModel
            = new LayoutAwarePage.ObservableDictionary<string, object>();
        public IObservableMap<string, object> DefaultViewModel { get { return _defaultViewModel; } }

        public UserPanel(User user) {
            InitializeComponent();

            DefaultViewModel["User"] = user;
            DefaultViewModel["HasAka"] = user.AlsoKnownAs.Length > 0;
            DefaultViewModel["CanModerate"] =
                user.Id != App.Connection.GetSession().Id
                && App.Connection.GetSession().IsModerator();
            DefaultViewModel["CanBan"] =
                user.Id != App.Connection.GetSession().Id
                && !user.IsModerator() || App.Connection.GetSession().IsOwner();
            DefaultViewModel["TrollBan"] =
                user.Role == "founder" && user.Role == "developer";
        }
    }
}
