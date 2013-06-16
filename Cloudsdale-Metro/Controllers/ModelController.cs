using System.Collections.Generic;
using System.Threading.Tasks;
using CloudsdaleLib.Models;
using CloudsdaleLib.Providers;

namespace Cloudsdale_Metro.Controllers {
    public class ModelController : IUserProvider, ICloudProvider {
        private readonly Dictionary<string, User> users = new Dictionary<string, User>();
        private readonly Dictionary<string, Cloud> clouds = new Dictionary<string, Cloud>();
        private SessionController sessionController { get { return App.Connection.SessionController; } }

        public async Task<User> GetUserAsync(string id) {
            if (id == sessionController.CurrentSession.Id) {
                return sessionController.CurrentSession;
            }

            if (!users.ContainsKey(id)) {
                var user = new User(id);
                await user.ForceValidate();
                users[id] = user;
            } else {
                await users[id].Validate();
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
            } else {
                users[id].Validate();
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

        public async Task<Cloud> UpdateCloudAsync(Cloud cloud) {
            if (!clouds.ContainsKey(cloud.Id)) {
                await cloud.ForceValidate();
                clouds[cloud.Id] = cloud;
            } else {
                cloud.CopyTo(clouds[cloud.Id]);
            }

            return clouds[cloud.Id];
        }

        public Cloud UpdateCloud(Cloud cloud) {
            if (!clouds.ContainsKey(cloud.Id)) {
                cloud.ForceValidate();
                clouds[cloud.Id] = cloud;
            } else {
                cloud.CopyTo(clouds[cloud.Id]);
            }

            return clouds[cloud.Id];
        }

        public Cloud GetCloud(string cloudId) {
            if (!clouds.ContainsKey(cloudId)) {
                var cloud = new Cloud(cloudId);
                cloud.ForceValidate();
                clouds[cloud.Id] = cloud;
            } else {
                clouds[cloudId].Validate();
            }

            return clouds[cloudId];
        }
    }
}
