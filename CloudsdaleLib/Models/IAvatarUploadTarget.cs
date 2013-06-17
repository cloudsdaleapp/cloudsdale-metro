using System.IO;
using System.Threading.Tasks;

namespace CloudsdaleLib.Models {
    /// <summary>
    /// Represents a resource which contains an avatar, and can have its avatar updated
    /// </summary>
    public interface IAvatarUploadTarget {
        Avatar Avatar { get; }
        Task UploadAvatar(Stream pictureStream, string mimeType);
    }
}
