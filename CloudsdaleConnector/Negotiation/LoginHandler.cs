using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudsdaleConnector.Negotiation {
    public abstract class LoginHandler {
        private const string LoginUri = "http://www.cloudsdale.org/v1/sessions";

        public abstract Task Login();

        public void SendLogin(string postData) {
            
        }
    }
}
