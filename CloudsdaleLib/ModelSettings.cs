using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace CloudsdaleLib {
    public static class ModelSettings {
        public const string InternalToken = "$2a$10$7Pfcv89Q9c/9WMAk6ySfhu";
        public static CoreDispatcher Dispatcher = null;
        public static DateTime AppLastSuspended = DateTime.Now;
    }
}
