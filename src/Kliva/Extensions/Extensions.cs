using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

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

    public static class MapControlExtensions
    {
        public static void ClearMap<T>(this MapControl map, MapElement element = null) where T : MapElement
        {
            if (element == null)
            {
                List<MapElement> mapElements = map.MapElements.Where(item => item is T).ToList();
                foreach (var item in mapElements)
                    map.MapElements.Remove(item);
            }
            else
                map.MapElements.Remove(element);
        }
    }
}
