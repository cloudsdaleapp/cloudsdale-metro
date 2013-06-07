using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Models;

namespace Cloudsdale_Metro.Models {
    public class LastSession {
        public string UserId;
        public Dictionary<string, DateTime> LastLogins = new Dictionary<string, DateTime>();
    }
}
