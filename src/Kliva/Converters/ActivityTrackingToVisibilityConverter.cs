using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Kliva.Models;

namespace Kliva.Converters
{
    public class ActivityTrackingToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ActivityTracking && parameter is ActivityTracking)
            {
                if ((ActivityTracking) value == (ActivityTracking) parameter)
                    return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
