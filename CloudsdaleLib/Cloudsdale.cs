using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Models;

namespace CloudsdaleLib {
    public class Cloudsdale {
        public static readonly Cloudsdale Instance = new Cloudsdale();

        public Session Session { get; set; }
    }
}
