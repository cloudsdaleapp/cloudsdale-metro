using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;
using Windows.UI.Core;

namespace Cloudsdale {
    internal static class Helpers {
        public static CoreDispatcher Dispatcher;

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            CheckAdditionalContent = false,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            ConstructorHandling = ConstructorHandling.Default,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            Error = OnJsonError,
        };

        public static event EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> JsonError; 
        static void OnJsonError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) {
            if (JsonError != null) JsonError(sender, args);
        }

        public static string Serialize(object obj) {
            return JsonConvert.SerializeObject(obj, JsonSettings);
        }

        public static async Task<string> SerializeAsync(object obj) {
            return await JsonConvert.SerializeObjectAsync(obj, Formatting.None, JsonSettings);
        }

        public static T Deserialize<T>(string data) {
            data = data.Trim().TrimStart('[').TrimEnd(']');
            return JsonConvert.DeserializeObject<T>(data, JsonSettings);
        }

        public static async Task<T> DeserializeAsync<T>(string data) {
            return await JsonConvert.DeserializeObjectAsync<T>(data, JsonSettings);
        }

        public static Task WaitAsync(WaitHandle handle) {
            var task = new Task(() => handle.WaitOne());
            task.Start();
            return task;
        }

        public static bool UIAccess {
            get { return Dispatcher.HasThreadAccess; }
        }

        public static async Task RunInUI(DispatchedHandler func, CoreDispatcherPriority priority) {
            await Dispatcher.RunAsync(priority, func);
        }

        public static async Task<StorageFile> GetDataFileAsync(string name) {
            var appData = ApplicationData.Current.RoamingFolder;
            var cdData = await appData.CreateFolderAsync("Cloudsdale", CreationCollisionOption.OpenIfExists);
            var dataFolder = await cdData.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            return await dataFolder.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
        }

        public static async Task<bool> DataFileExists(string name) {
            var appData = ApplicationData.Current.RoamingFolder;
            var cdData = await appData.CreateFolderAsync("Cloudsdale", CreationCollisionOption.OpenIfExists);
            var dataFolder = await cdData.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            var files = await dataFolder.GetFilesAsync();
            return files.Any(file => file.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task DeleteDataFileAsync(string name) {
            var file = await GetDataFileAsync(name);
            await file.DeleteAsync(StorageDeleteOption.Default);
        }

        public static async Task SaveAsGZippedJson(StorageFile file, object jsonobj) {
            using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
            using (var gzip = new GZipStream(fs.GetOutputStreamAt(0).AsStreamForWrite(), CompressionLevel.Optimal))
            using (var sw = new StreamWriter(gzip)) {
                await sw.WriteAsync(await SerializeAsync(jsonobj));
            }
        }

        public static async Task<T> ReadGZippedJson<T>(StorageFile file) {
            using (var fs = await file.OpenAsync(FileAccessMode.Read))
            using (var gzip = new GZipStream(fs.GetInputStreamAt(0).AsStreamForRead(), CompressionMode.Decompress))
            using (var sr = new StreamReader(gzip)) {
                return await DeserializeAsync<T>(await sr.ReadToEndAsync());
            }
        }

        public static string ParseLiteral(string input) {
            var result = new StringBuilder(input.Length);
            for (var i = 0; i < input.Length; ++i) {
                var c = input[i];
                if (c != '\\') {
                    result.Append(c);
                    continue;
                }
                if (++i >= input.Length) {
                    result.Append(c);
                    break;
                }
                c = input[i];
                switch (c) {
                    case 'n':
                        result.Append('\n');
                        break;
                    case 'r':
                        result.Append('\r');
                        break;
                    case 't':
                        result.Append('\t');
                        break;
                    case '\\':
                        result.Append('\\');
                        break;
                }
            }
            return result.ToString();
        }
    }

    public delegate void EventHandler<in TSender, in TArgs>(TSender sender, TArgs e);
}
