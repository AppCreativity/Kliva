namespace Kliva.Models
{
    /// <summary>
    /// This class holds information about the Strava API limits of your application.
    /// </summary>
    public class Limit
    {
        /// <summary>
        /// The short term limit. The default rate limit allows 600 requests every 15 minutes.
        /// </summary>
        public int ShortTerm { get; set; }

        /// <summary>
        /// The long term limit. The default rate limit allows 30.000 requests every day.
        /// </summary>
        public int LongTerm { get; set; }

        /// <summary>
        /// Initializes a new instance of the Limit class.
        /// </summary>
        /// <param name="shortTerm">The short term limit that was read from a header in a WebResponse.</param>
        /// <param name="longTerm">The long term limit that was read from a header in a WebResponse.</param>
        public Limit(int shortTerm, int longTerm)
        {
            ShortTerm = shortTerm;
            LongTerm = longTerm;
        }
    }
}
