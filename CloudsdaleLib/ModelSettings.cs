using System;
using Windows.UI.Core;

namespace CloudsdaleLib {
    public static class ModelSettings {
        public static CoreDispatcher Dispatcher = null;
        public static DateTime AppLastSuspended = DateTime.Now;
    }
}
