using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI.Core;

namespace CloudsdaleConnector {
    public static class Helpers {
        internal static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            CheckAdditionalContent = false,
            ConstructorHandling = ConstructorHandling.Default,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DefaultValueHandling = DefaultValueHandling.Populate,
            Formatting = Formatting.None,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
        };

        public static Task<T> DeserializeAsync<T>(string data) {
            return JsonConvert.DeserializeObjectAsync<T>(data, Settings);
        }

        public static Task<string> SerializeAsync(object o) {
            return JsonConvert.SerializeObjectAsync(o);
        }

        public static CoreDispatcher Dispatcher { get; set; }

        public static bool UIAccess {
            get { return Dispatcher.HasThreadAccess; }
        }
    }
}
