using System;
using System.Globalization;
using Windows.UI.Xaml.Data;
using Kliva.Helpers;

namespace Kliva.Converters
{
    public class RelativeTimeConverter : IValueConverter
    {
        //TODO: Glenn - Change back to use of Resources for translation like WP8.1 app

        private const string XMonthsAgo_2To4 = "{0} months ago";
        private const string XMonthsAgo_5To12 = "{0} months ago";

        private const string XWeeksAgo_2To4 = "{0} weeks ago";

        private const string XHoursAgo_2To4 = "{0} hours ago";
        private const string XHoursAgo_EndsIn1Not11 = "{0} hours ago";
        private const string XHoursAgo_EndsIn2To4Not12To14 = "{0} hours ago";
        private const string XHoursAgo_Other = "{0} hours ago";

        private const string XMinutesAgo_2To4 = "{0} minutes ago";
        private const string XMinutesAgo_EndsIn1Not11 = "{0} minutes ago";
        private const string XMinutesAgo_EndsIn2To4Not12To14 = "{0} minutes ago";
        private const string XMinutesAgo_Other = "{0} minutes ago";

        private const string XSecondsAgo_2To4 = "{0} seconds ago";
        private const string XSecondsAgo_EndsIn1Not11 = "{0} seconds ago";
        private const string XSecondsAgo_EndsIn2To4Not12To14 = "{0} seconds ago";
        private const string XSecondsAgo_Other = "{0} seconds ago";

        /// <summary>
        /// A minute defined in seconds.
        /// </summary>
        private const double Minute = 60.0;

        /// <summary>
        /// An hour defined in seconds.
        /// </summary>
        private const double Hour = 60.0 * Minute;

        /// <summary>
        /// A day defined in seconds.
        /// </summary>
        private const double Day = 24 * Hour;

        /// <summary>
        /// A week defined in seconds.
        /// </summary>
        private const double Week = 7 * Day;

        /// <summary>
        /// A month defined in seconds.
        /// </summary>
        private const double Month = 30.5 * Day;

        /// <summary>
        /// A year defined in seconds.
        /// </summary>
        private const double Year = 365 * Day;

        /// <summary>
        /// Four different strings to express hours in plural.
        /// </summary>
        private string[] PluralHourStrings;

        /// <summary>
        /// Four different strings to express minutes in plural.
        /// </summary>
        private string[] PluralMinuteStrings;

        /// <summary>
        /// Four different strings to express seconds in plural.
        /// </summary>
        private string[] PluralSecondStrings;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            PluralHourStrings = new string[4] { XHoursAgo_2To4, XHoursAgo_EndsIn1Not11, XHoursAgo_EndsIn2To4Not12To14, XHoursAgo_Other };

            PluralMinuteStrings = new string[4] { XMinutesAgo_2To4, XMinutesAgo_EndsIn1Not11, XMinutesAgo_EndsIn2To4Not12To14, XMinutesAgo_Other };

            PluralSecondStrings = new string[4] { XSecondsAgo_2To4, XSecondsAgo_EndsIn1Not11, XSecondsAgo_EndsIn2To4Not12To14, XSecondsAgo_Other };

            string result;

            DateTime given;
            DateTime.TryParse((string)value, out given);
            given = given.ToLocalTime();

            DateTime current = DateTime.Now;

            TimeSpan difference = current - given;

            if (DateTimeFormatHelper.IsFutureDateTime(current, given))
            {
                // Future dates and times are not supported, but to prevent crashing an app
                // if the time they receive from a server is slightly ahead of the phone's clock
                // we'll just default to the minimum, which is "2 seconds ago".
                result = GetPluralTimeUnits(2, PluralSecondStrings);
            }

            if (difference.TotalSeconds > Year)
            {
                // "over a year ago"
                result = "over a year ago";
            }
            else if (difference.TotalSeconds > (1.5 * Month))
            {
                // "x months ago"
                int nMonths = (int)((difference.TotalSeconds + Month / 2) / Month);
                result = GetPluralMonth(nMonths);
            }
            else if (difference.TotalSeconds >= (3.5 * Week))
            {
                // "about a month ago"
                result = "about a month ago";
            }
            else if (difference.TotalSeconds >= Week)
            {
                int nWeeks = (int)(difference.TotalSeconds / Week);
                if (nWeeks > 1)
                {
                    // "x weeks ago"
                    result = string.Format(XWeeksAgo_2To4, nWeeks.ToString());
                }
                else
                {
                    // "about a week ago"
                    result = "about a week ago";
                }
            }
            else if (difference.TotalSeconds >= (5 * Day))
            {
                // "last <dayofweek>"    
                result = GetLastDayOfWeek(given.DayOfWeek);
            }
            else if (difference.TotalSeconds >= Day)
            {
                // "on <dayofweek>"
                result = GetOnDayOfWeek(given.DayOfWeek);
            }
            else if (difference.TotalSeconds >= (2 * Hour))
            {
                // "x hours ago"
                int nHours = (int)(difference.TotalSeconds / Hour);
                result = GetPluralTimeUnits(nHours, PluralHourStrings);
            }
            else if (difference.TotalSeconds >= Hour)
            {
                // "about an hour ago"
                result = "about an hour ago";
            }
            else if (difference.TotalSeconds >= (2 * Minute))
            {
                // "x minutes ago"
                int nMinutes = (int)(difference.TotalSeconds / Minute);
                result = GetPluralTimeUnits(nMinutes, PluralMinuteStrings);
            }
            else if (difference.TotalSeconds >= Minute)
            {
                // "about a minute ago"
                result = "about a minute ago";
            }
            else
            {
                // "x seconds ago" or default to "2 seconds ago" if less than two seconds.
                int nSeconds = ((int)difference.TotalSeconds > 1.0) ? (int)difference.TotalSeconds : 2;
                result = GetPluralTimeUnits(nSeconds, PluralSecondStrings);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a localized text string to express months in plural.
        /// </summary>
        /// <param name="month">Number of months.</param>
        /// <returns>Localized text string.</returns>
        private static string GetPluralMonth(int month)
        {
            if (month >= 2 && month <= 4)
            {
                return string.Format(XMonthsAgo_2To4, month.ToString());
            }
            else if (month >= 5 && month <= 12)
            {
                return string.Format(XMonthsAgo_5To12, month.ToString());
            }

            return null;
        }

        /// <summary>
        /// Returns a localized text string to express time units in plural.
        /// </summary>
        /// <param name="units">
        /// Number of time units, e.g. 5 for five months.
        /// </param>
        /// <param name="resources">
        /// Resources related to the specified time unit.
        /// </param>
        /// <returns>Localized text string.</returns>
        private static string GetPluralTimeUnits(int units, string[] resources)
        {
            int modTen = units % 10;
            int modHundred = units % 100;

            if (units <= 1)
            {
                //throw new ArgumentException(Properties.Resources.InvalidNumberOfTimeUnits);
            }
            else if (units >= 2 && units <= 4)
            {
                return string.Format(resources[0], units.ToString());
            }
            else if (modTen == 1 && modHundred != 11)
            {
                return string.Format(resources[1], units.ToString());
            }
            else if ((modTen >= 2 && modTen <= 4) && !(modHundred >= 12 && modHundred <= 14))
            {
                return string.Format(resources[2], units.ToString());
            }
            else
            {
                return string.Format(resources[3], units.ToString());
            }

            return null;
        }

        /// <summary>
        /// Returns a localized text string for the "ast" + "day of week as {0}".
        /// </summary>
        /// <param name="dow">Last Day of week.</param>
        /// <returns>Localized text string.</returns>
        private static string GetLastDayOfWeek(DayOfWeek dow)
        {
            string result;

            switch (dow)
            {
                case DayOfWeek.Monday:
                    result = "last Monday";
                    break;
                case DayOfWeek.Tuesday:
                    result = "last Tuesday";
                    break;
                case DayOfWeek.Wednesday:
                    result = "last Wednesday";
                    break;
                case DayOfWeek.Thursday:
                    result = "last _Thursday";
                    break;
                case DayOfWeek.Friday:
                    result = "last Friday";
                    break;
                case DayOfWeek.Saturday:
                    result = "last Saturday";
                    break;
                case DayOfWeek.Sunday:
                    result = "last Sunday";
                    break;
                default:
                    result = "last Sunday";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Returns a localized text string to express "on {0}"
        /// where {0} is a day of the week, e.g. Sunday.
        /// </summary>
        /// <param name="dow">Day of week.</param>
        /// <returns>Localized text string.</returns>
        private static string GetOnDayOfWeek(DayOfWeek dow)
        {
            string result;

            switch (dow)
            {
                case DayOfWeek.Monday:
                    result = "on Monday";
                    break;
                case DayOfWeek.Tuesday:
                    result = "on Tuesday";
                    break;
                case DayOfWeek.Wednesday:
                    result = "on Wednesday";
                    break;
                case DayOfWeek.Thursday:
                    result = "on Thursday";
                    break;
                case DayOfWeek.Friday:
                    result = "on Friday";
                    break;
                case DayOfWeek.Saturday:
                    result = "on Saturday";
                    break;
                case DayOfWeek.Sunday:
                    result = "on Sunday";
                    break;
                default:
                    result = "on Sunday";
                    break;
            }

            return result;
        }
    }
}
