using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Helpers;
using CloudsdaleLib.Providers;
using Windows.UI.Popups;

namespace Cloudsdale_Metro.Controllers {
    public class ErrorController : IModelErrorProvider {
        public object LastError;

        public async Task OnError<T>(WebResponse<T> response) {
            LastError = response;
            var message = response.Errors.Aggregate(
                response.Flash.Message + "\n", 
                (current, error) => current + "\n  - " + error.Node + " '" 
                    + error.NodeValue + "' " + error.Message);
            var dialog = new MessageDialog(message, response.Flash.Title);
            await dialog.ShowAsync();
        }
    }
}
