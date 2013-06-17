using CloudsdaleLib.Models;

namespace CloudsdaleLib.Providers {
    /// <summary>
    /// Provides internal models access to the backing cache of users
    /// </summary>
    public interface IUserProvider {
        User GetUser(string userId);
    }

    class DefaultUserProvider : IUserProvider {
        public User GetUser(string userId) {
            return null;
        }
    }
}
