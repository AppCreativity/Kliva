using System;
using System.Globalization;

namespace Kliva.Helpers
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
}
