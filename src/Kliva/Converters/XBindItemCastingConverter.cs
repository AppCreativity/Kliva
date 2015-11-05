using System;
using Windows.UI.Xaml.Data;

namespace Kliva.Converters
{
    public class XBindItemCastingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
