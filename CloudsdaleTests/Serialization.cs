using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CloudsdaleConnector;
using CloudsdaleConnector.DataModels;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace CloudsdaleTests {
    [TestClass]
    public class Serialization {
        public const string ExampleUser =
            @"{""name"":""Nife"",""time_zone"":null,""member_since"":""2012-06-10T00:52:30Z"",
               ""suspended_until"":null,""reason_for_suspension"":null,""id"":""4fd3efcecff4e878a80013bc"",
               ""avatar"":{
                ""normal"":""http://c775850.r50.cf2.rackcdn.com/avatars/4fd3efcecff4e878a80013bc/3d36371760-avatar.png"",
                ""mini"":""http://c775850.r50.cf2.rackcdn.com/avatars/4fd3efcecff4e878a80013bc/mini_3d36371760-avatar.png"",
                ""thumb"":""http://c775850.r50.cf2.rackcdn.com/avatars/4fd3efcecff4e878a80013bc/thumb_3d36371760-avatar.png"",
                ""preview"":""http://c775850.r50.cf2.rackcdn.com/avatars/4fd3efcecff4e878a80013bc/preview_3d36371760-avatar.png"",
                ""chat"":""http://c775850.r50.cf2.rackcdn.com/avatars/4fd3efcecff4e878a80013bc/chat_3d36371760-avatar.png""},
                ""is_registered"":true,""is_transient"":false,""is_banned"":false,""is_member_of_a_cloud"":true,
                ""has_an_avatar"":true,""has_read_tnc"":true,""role"":""moderator"",""prosecutions"":[]}";

        [TestMethod]
        public async Task DeserializeUser() {
            var user = await Helpers.DeserializeAsync<User>(ExampleUser);
            Debug.Assert(user.Id == "4fd3efcecff4e878a80013bc", "The Id wasn't 4fd3efcecff4e878a80013bc...");
            Debug.Assert(user.Name == "Nife", "The name wasn't Nife...");
            Debug.Assert(user.Role == "moderator", "The role wasn't moderator...");
            Debug.Assert(user.JoinDate != null && user.JoinDate.Value.Date == DateTime.Parse("2012-06-10").Date, 
                                                  "Join date wasn't 2012-06-10...");
        }
    }
}
