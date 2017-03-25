using System;
using Windows.UI.Xaml.Data;
using Kliva.Helpers;

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
            string translatedValue = TranslationHelper.GetTranslation((string)parameter);
            string result = string.IsNullOrEmpty(translatedValue) ? (string)parameter : translatedValue;

            if (value is int)
            {
                int amount = (int) value;
                if (amount != 0)
                {
                    return $"{result} ({amount})";
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
