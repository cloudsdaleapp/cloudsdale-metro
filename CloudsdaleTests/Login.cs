using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleConnector.Negotiation;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudsdaleTests {
    [TestClass]
    public class Login {
        private const string TestEmail = "connorcpu@live.com";
        private const string TestPassword = "notmypassword";

        [TestMethod]
        public async Task EmailLogin() {
            LoginHandler handler = new EmailLogin {
                Email = TestEmail,
                Password = TestPassword,
            };
            var response = await handler.Login();
            Debug.WriteLine(response);
        }
    }
}
