using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CloudsdaleConnector.Negotiation {
    public class EmailLogin : LoginHandler {
        public string Email { get; set; }
        public string Password { get; set; }

        public override async Task<JObject> Login() {
            try {
                var request = new JObject();
                request.Root["email"] = Email;
                request.Root["password"] = Password;
                var requestString = request.ToString();
                var response = await SendLogin(requestString);
                response.Root["__success"] = true;
                return response;
            } catch (WebException ex) {
                var respobj = new JObject();
                respobj.Root["__success"] = false;
                respobj.Root["status"] = ex.Response.Headers["Status"];
                return respobj;
            }
        }
    }
}
