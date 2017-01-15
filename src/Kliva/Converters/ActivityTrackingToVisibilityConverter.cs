using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Kliva.Extensions;
using Kliva.Models;

namespace Kliva.Converters
{
    public class ActivityTrackingToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ActivityTracking && parameter is string)
            {
                if ((ActivityTracking) value != Enum<ActivityTracking>.Parse((string)parameter))
                    return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseActivityTrackingToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ActivityTracking && parameter is string)
            {
                if ((ActivityTracking)value == Enum<ActivityTracking>.Parse((string)parameter))
                    return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
