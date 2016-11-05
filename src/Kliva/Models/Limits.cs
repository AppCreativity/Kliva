using System;

namespace Kliva.Models
{
    /// <summary>
    /// This class contains information about the Strava API limits and how much requests are consumed by your application.
    /// </summary>
    public static class Limits
    {
        /// <summary>
        /// UsageChanged is raised whenever a web request is sent to the Strava servers and a response is received.
        /// </summary>
        public static event EventHandler<UsageChangedEventArgs> UsageChanged;

        private static Usage _usage;
        private static Limit _limit;

        /// <summary>
        /// The short- and long-term usage of your application.
        /// </summary>
        public static Usage Usage
        {
            get { return _usage ?? (_usage = new Usage(0, 0)); }
            set
            {
                _usage = value;

                UsageChanged?.Invoke(null, new UsageChangedEventArgs(value.ShortTerm, value.LongTerm));
            }
        }

        /// <summary>
        /// The short- and long-term limit of your application.
        /// </summary>
        public static Limit Limit
        {
            get { return _limit ?? (_limit = new Limit(0, 0)); }
            set
            {
                _limit = value;
            }
        }
    }
}
