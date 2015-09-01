namespace Kliva.Models
{
    /// <summary>
    /// Class that holds information about the API usage.
    /// </summary>
    public class UsageChangedEventArgs
    {
        /// <summary>
        /// The Usage.
        /// </summary>
        public Usage Usage { get; set; }

        /// <summary>
        /// Initializes a new instance of the UsageChangedEventArgs class.
        /// </summary>
        /// <param name="shortTerm">Short term limit.</param>
        /// <param name="longTerm">Long term limit.</param>
        public UsageChangedEventArgs(int shortTerm, int longTerm)
        {
            Usage = new Usage(shortTerm, longTerm);
        }
    }

    /// <summary>
    /// This class holds information about the Strava API usage of your application.
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// The usage of the short term limit. Default short term limit is 600 requests per 15 minutes.
        /// </summary>
        public int ShortTerm { get; set; }

        /// <summary>
        /// The usage of the long term limit. Default long term limit is 30.000 requests a day.
        /// </summary>
        public int LongTerm { get; set; }

        /// <summary>
        /// Initializes a new instances of the Usage class.
        /// </summary>
        /// <param name="shortTerm">The short term usage.</param>
        /// <param name="longTerm">The long term usage.</param>
        public Usage(int shortTerm, int longTerm)
        {
            ShortTerm = shortTerm;
            LongTerm = longTerm;
        }
    }
}
