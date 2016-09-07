using System;
using Windows.UI.Xaml.Data;
using Kliva.Models;

namespace Kliva.Converters
{
    public class ActivityTrackingToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ActivityTracking)
            {
                switch ((ActivityTracking)value)
                {
                    case ActivityTracking.Idle:
                        return "start tracking";
                    case ActivityTracking.Recording:
                        return "stop tracking";
                    default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
