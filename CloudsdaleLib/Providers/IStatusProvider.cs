using CloudsdaleLib.Models;

namespace CloudsdaleLib.Providers {
    public interface IStatusProvider {
        Status StatusForUser(string userId);
    }

    internal class DefaultStatusProvider : IStatusProvider {
        public Status StatusForUser(string userId) {
            return Status.offline;
        }
    }
}
