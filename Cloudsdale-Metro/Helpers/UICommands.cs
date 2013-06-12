using System;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace Cloudsdale_Metro.Helpers {

    public class OpenLinkCommand : ICommand {
        public bool CanExecute(object parameter) {
            return true;
        }

        public async void Execute(object parameter) {
            await Launcher.LaunchUriAsync((Uri)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }

    public class CopyLinkCommand : ICommand {
        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            var package = new DataPackage();
            package.SetText(parameter.ToString());
            Clipboard.SetContent(package);
        }

        public event EventHandler CanExecuteChanged;
    }
}
