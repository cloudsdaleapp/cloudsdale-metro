using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Models;

namespace CloudsdaleLib.Providers {
    public interface IStatusProvider {
        Status StatusForUser(string userId);
    }

    internal class DefaultStatusProvider : IStatusProvider {
        public Status StatusForUser(string userId) {
            return Status.Offline;
        }
    }
}
