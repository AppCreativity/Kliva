using System;
using Windows.UI.Xaml.Data;

namespace Kliva.Converters
{
    public class SecondsToTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var timeSpan = TimeSpan.FromSeconds((int)value);

            string answer;

            //TODO: Glenn - use translated resources
            if (timeSpan.Days != 0)
                answer = $"{timeSpan.Days}d {timeSpan.Hours:D2}h {timeSpan.Minutes:D2}m";
            else
                answer = $"{timeSpan.Hours}h {timeSpan.Minutes:D2}m";

            return parameter == null ? answer : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
