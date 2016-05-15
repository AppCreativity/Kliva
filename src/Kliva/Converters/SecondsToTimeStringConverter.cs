using System;
using Windows.UI.Xaml.Data;

namespace Kliva.Converters
{
    public class SecondsToTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var timeSpan = TimeSpan.FromSeconds((int)value);

            string answer = string.Empty;

            //TODO: Glenn - use translated resources
            if (string.IsNullOrEmpty(answer) && timeSpan.Days != 0)
                answer = $"{timeSpan.Days}d {timeSpan.Hours:D2}h {timeSpan.Minutes:D2}m";

            if(string.IsNullOrEmpty(answer) && timeSpan.Hours != 0)
                answer = $"{timeSpan.Hours}h {timeSpan.Minutes:D2}m";

            if (string.IsNullOrEmpty(answer))
                answer = $"{timeSpan.Minutes}m {timeSpan.Seconds:D2}s";

            return parameter == null ? answer : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
