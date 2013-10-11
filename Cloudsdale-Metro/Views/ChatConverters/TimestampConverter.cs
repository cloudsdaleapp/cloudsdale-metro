using System;
using Windows.UI.Xaml.Data;

namespace Cloudsdale_Metro.Views.ChatConverters {
    public class TimestampConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var time = (DateTime)value;
            time = time.ToLocalTime();
            return time.ToString(time > DateTime.Now.AddDays(-1) ? "T" : "G");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }
}
