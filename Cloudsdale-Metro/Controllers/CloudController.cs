using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Models;
using CloudsdaleLib.Providers;

namespace Cloudsdale_Metro.Controllers {
    public class CloudController : IStatusProvider {
        private readonly Dictionary<string, Status> userStatuses = new Dictionary<string, Status>(); 

        public Status StatusForUser(string userId) {
            return Status.Offline;
        }
    }
}
