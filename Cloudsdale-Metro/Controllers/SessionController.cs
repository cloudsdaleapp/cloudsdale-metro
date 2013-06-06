using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CloudsdaleLib.Models;
using Cloudsdale_Metro.Helpers;
using Cloudsdale_Metro.Models;
using Newtonsoft.Json;
using Windows.Storage;

namespace Cloudsdale_Metro.Controllers {
    public class SessionController {
        private Session session;
        private LastSession lastSession;
        private readonly List<Session> pastSessions = new List<Session>(); 
        private readonly Regex userSessionPattern = new Regex(@"^session_([0-9a-f]+)\.json$", RegexOptions.IgnoreCase);

        public IReadOnlyList<Session> PastSessions {
            get { return new ReadOnlyCollection<Session>(pastSessions); }
        }

        public async Task LoadSession() {
            var storage = ApplicationData.Current.RoamingFolder;
            var sessionFolder = await storage.EnsureFolderExists("session");
            if (!await sessionFolder.FileExists("session.json")) {
                lastSession = new LastSession();
                session = null;

                await SaveSession();
                return;
            }

            var lastSessionFile = await sessionFolder.GetFileAsync("session.json");
            lastSession = await JsonConvert.DeserializeObjectAsync<LastSession>(await lastSessionFile.ReadAllText());

            if (lastSession.UserId == null) {
                session = null;

                await SaveSession();
                return;
            }

            pastSessions.Clear();
            var sessionFiles = (await sessionFolder.GetFilesAsync()).Where(file => userSessionPattern.IsMatch(file.Name));
            foreach (var sessionFile in sessionFiles) {
                pastSessions.Add(await JsonConvert.DeserializeObjectAsync<Session>(await sessionFile.ReadAllText()));
            }
        }

        public async Task SaveSession() {
            if (lastSession == null) return;

            var storage = ApplicationData.Current.RoamingFolder;
            var sessionFolder = await storage.EnsureFolderExists("session");

            var lastSessionFile = await sessionFolder.CreateFileAsync("session.json", CreationCollisionOption.ReplaceExisting);
            await lastSessionFile.SaveAllText(await JsonConvert.SerializeObjectAsync(lastSession));
        }
    }
}
