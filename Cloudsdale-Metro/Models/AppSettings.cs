using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using CloudsdaleLib.Models;
using Cloudsdale_Metro.Helpers;
using Newtonsoft.Json;

namespace Cloudsdale_Metro.Models {
    [JsonObject(MemberSerialization.OptIn)]
    public class AppSettings : CloudsdaleModel {
        public static readonly AppSettings Settings = new AppSettings();
        private bool _displayNotifications;

        public AppSettings() {
            PropertyChanged += (sender, args) => Save().Wait();
        }

        [JsonProperty]
        public bool DisplayNotifications {
            get { return _displayNotifications; }
            set {
                if (value.Equals(_displayNotifications)) return;
                _displayNotifications = value;
                OnPropertyChanged();
            }
        }

        public static async Task Load() {
            var storage = ApplicationData.Current.RoamingFolder;
            if (!await storage.FileExists("settings.json")) {
                return;
            }

            var settingsFile = await storage.GetFileAsync("settings.json");
            var settingsData = await settingsFile.ReadAllText();
            var settings = await JsonConvert.DeserializeObjectAsync<AppSettings>(settingsData);
            if (settings == null) return;
            settings.CopyTo(Settings);
        }

        public static async Task Save() {
            var storage = ApplicationData.Current.RoamingFolder;
            var settingsFile = await storage.CreateFileAsync("settings.json", CreationCollisionOption.ReplaceExisting);
            var settingsData = await JsonConvert.SerializeObjectAsync(Settings);
            await settingsFile.SaveAllText(settingsData);
        }
    }
}
