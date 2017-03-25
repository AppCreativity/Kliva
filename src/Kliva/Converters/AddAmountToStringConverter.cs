using System;
using Windows.UI.Xaml.Data;

namespace Kliva.Converters
{
    public class AddAmountToStringConverter : IValueConverter
    {
        /// <summary>
        /// Add the bound amount to a given string, e.g. Kudos (2)
        /// </summary>
        /// <param name="value">Amount</param>
        /// <param name="targetType"></param>
        /// <param name="parameter">Initial string to expand</param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int)
            {
                int amount = (int) value;
                if (amount != 0)
                    return $"{parameter} ({amount})";
            }

            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
