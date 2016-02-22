using System;
using Windows.UI.Xaml.Data;
using Kliva.Helpers;
using Kliva.Models;

namespace Kliva.Converters
{
    public class DistanceUnitToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DistanceUnitType)
                return UnitConverter.DistanceSymbols[(int) (DistanceUnitType) value];

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
