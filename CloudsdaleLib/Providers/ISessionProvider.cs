using CloudsdaleLib.Models;

namespace CloudsdaleLib.Providers {
    public interface ISessionProvider {
        Session CurrentSession { get; }
    }
    internal class DefaultSessionProvider : ISessionProvider {
        public Session CurrentSession { get { return null; } }
    }
}
