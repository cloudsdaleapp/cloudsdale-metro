using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Cloudsdale_Metro.Views.ChatConverters {
    public class TimestampConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var time = (DateTime)value;
            return time.ToString(time.Date == DateTime.Today ? "T" : "G");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
