using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Models;

namespace Cloudsdale_Metro.Controllers {
    public class UserController {
        private readonly Dictionary<string, User> users = new Dictionary<string, User>();

        public async Task<User> GetUserAsync(string id) {
            if (!users.ContainsKey(id)) {
                var user = new User(id);
                await user.ForceValidate();
                users[id] = user;
            }
            return users[id];
        }

        public User GetUser(string id) {
            if (!users.ContainsKey(id)) {
                var user = new User(id);
                user.ForceValidate();
                users[id] = user;
            }
            return users[id];
        }
    }
}
