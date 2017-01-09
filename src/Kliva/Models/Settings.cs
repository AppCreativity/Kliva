namespace Kliva.Models
{
    /// <summary>
    /// Application settings, stored locally
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Strava access token for the logged-in user
        /// </summary>
        public string StravaAccessToken { get; set; }

        public DistanceUnitType DistanceUnitType { get; set; }

        public ActivityFeedFilter ActivityFeedFilter { get; set; }

        public ActivitySort ActivitySort { get; set; }

        public AppVersion AppVersion { get; set; }
    }
}
