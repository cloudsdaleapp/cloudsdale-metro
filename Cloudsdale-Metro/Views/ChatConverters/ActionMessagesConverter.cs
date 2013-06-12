using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CloudsdaleLib.Models;
using Windows.UI.Xaml.Data;

namespace Cloudsdale_Metro.Views.ChatConverters {
    public class ActionMessagesConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var messages = (string[])value;
            messages[0] = Message.SlashMeFormat.Replace(messages[0], "");
            return messages;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
