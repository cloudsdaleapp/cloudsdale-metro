using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudsdale.Models.Json;
using MetroFayeClient;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Cloudsdale.Controllers.Data {
    public static class ConnectionController {
        #region Members
        public static FayeConnector Faye = new FayeConnector();
        public static LoggedInUser CurrentUser;
        public static Cloud CurrentCloud;
        private static readonly Dictionary<string, MessageProcessor> MessageProcessors =
                            new Dictionary<string, MessageProcessor>();
        public static List<string> CloudOrder = null;
        #endregion

        #region User
        public static async Task SaveUserAsync() {
            await Helpers.SaveAsGZippedJson(
                await Helpers.GetDataFileAsync("CurrentUser.json"),
                CurrentUser);

        }

        public static async Task<bool> LoadUserAsync() {
            if (await Helpers.DataFileExists("CloudOrder.json")) {
                CloudOrder = await Helpers.ReadGZippedJson<List<string>>(
                             await Helpers.GetDataFileAsync("CurrentUser.json"));
            }
            if (await Helpers.DataFileExists("CurrentUser.json")) {
                CurrentUser = await Helpers.ReadGZippedJson<LoggedInUser>(
                              await Helpers.GetDataFileAsync("CurrentUser.json"));
                return true;
            }
            return false;
        }

        #endregion

        #region Messages, Drops, And Subscription
        public static async Task<MessageProcessor> Subscribe(string cloud) {
            return null;
        }

        #region Messages
        public static MessageProcessor GetMessages(string cloud) {
            return null;
        }
        #endregion
        #endregion
    }
}
