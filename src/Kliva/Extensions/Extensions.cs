using System;
using System.Globalization;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Kliva.Extensions
{
    /// <summary>
    /// Now you can use Generics to have cleaner code when enum parsing!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <example>
    /// CT.Organ organNewParse = Enum<CT.Organ>.Parse("LENS");
    /// </example>
    public static class Enum<T>
    {
        public static T Parse(string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime GetFirstDayOfTheWeek(this DateTime dateTime)
        {
            DayOfWeek firstDay = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dateTime.Date;

            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }
    }

    public static class ContentDialogExtensions
    {
        public static void AdjustSize(this ContentDialog dialog)
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            if (size.Width > 1000.0)
            {
                dialog.MinWidth = 500;
                dialog.MinHeight = 250;
            }
            else
                dialog.MinWidth = dialog.MinHeight = 300;
        }
    }
}
