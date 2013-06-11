using System.Collections.Generic;
using System.Threading.Tasks;
using CloudsdaleLib.Models;

namespace Cloudsdale_Metro.Controllers {
    public class UserController {
        private readonly Dictionary<string, User> users = new Dictionary<string, User>();
        private SessionController sessionController { get { return App.Connection.Session; } }

        public async Task<User> GetUserAsync(string id) {
            if (id == sessionController.CurrentSession.Id) {
                return sessionController.CurrentSession;
            }

            if (!users.ContainsKey(id)) {
                var user = new User(id);
                await user.ForceValidate();
                users[id] = user;
            }
            return users[id];
        }

        public User GetUser(string id) {
            if (id == sessionController.CurrentSession.Id) {
                return sessionController.CurrentSession;
            }

            if (!users.ContainsKey(id)) {
                var user = new User(id);
                user.ForceValidate();
                users[id] = user;
            }
            return users[id];
        }

        public async Task<User> UpdateDataAsync(User user) {
            if (user.Id == sessionController.CurrentSession.Id) {
                var session = sessionController.CurrentSession;
                user.CopyTo(session);
                return session;
            }

            if (!users.ContainsKey(user.Id)) {
                await user.ForceValidate();
                users[user.Id] = user;
            } else {
                user.CopyTo(users[user.Id]);
            }
            return users[user.Id];
        }
    }
}
