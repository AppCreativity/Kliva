using System;
using Windows.UI.Xaml.Data;
using Kliva.Helpers;
using Kliva.Models;

namespace Kliva.Converters
{
    public class SpeedUnitToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is SpeedUnit)
                return UnitConverter.SpeedSymbols[(int)(SpeedUnit)value];

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
