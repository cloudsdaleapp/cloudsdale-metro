using CloudsdaleLib.Models;

namespace CloudsdaleLib.Providers {
    /// <summary>
    /// Provides internal models access to the currently logged in session
    /// </summary>
    public interface ISessionProvider {
        Session CurrentSession { get; }
    }
    internal class DefaultSessionProvider : ISessionProvider {
        public Session CurrentSession { get { return null; } }
    }
}
