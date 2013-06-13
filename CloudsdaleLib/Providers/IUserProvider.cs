using CloudsdaleLib.Models;

namespace CloudsdaleLib.Providers {
    public interface IUserProvider {
        User GetUser(string userId);
    }

    class DefaultUserProvider : IUserProvider {
        public User GetUser(string userId) {
            return null;
        }
    }
}
