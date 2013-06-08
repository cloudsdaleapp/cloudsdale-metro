using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Helpers;

namespace CloudsdaleLib.Models {
    public interface IAvatarUploadTarget {
        Avatar Avatar { get; }
        Task UploadAvatar(Stream pictureStream, string mimeType);
    }
}
