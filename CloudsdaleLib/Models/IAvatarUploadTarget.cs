using System.IO;
using System.Threading.Tasks;

namespace CloudsdaleLib.Models {
    public interface IAvatarUploadTarget {
        Avatar Avatar { get; }
        Task UploadAvatar(Stream pictureStream, string mimeType);
    }
}
