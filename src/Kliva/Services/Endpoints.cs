namespace Kliva.Services
{
    /// <summary>
    /// This static class contains the Strava API endpoint Urls.
    /// </summary>
    public static class Endpoints
    {
        /// <summary>
        /// Url to the Activity endpoint used for the currently authenticated athlete.
        /// </summary>
        public const string Activity = "https://www.strava.com/api/v3/activities";
        /// <summary>
        /// Url to the Activity endpoint used for other athletes than the currently authenticated one.
        /// </summary>
        public const string Activities = "https://www.strava.com/api/v3/athlete/activities";
        /// <summary>
        /// Url to the Followers endpoint.
        /// </summary>
        public const string ActivitiesFollowers = "https://www.strava.com/api/v3/activities/following";
        /// <summary>
        /// Url to the Athlete endpoint used for the currently authenticated athlete.
        /// </summary>
        public const string Athlete = "https://www.strava.com/api/v3/athlete";
        /// <summary>
        /// Url to the Athlete endpoint used for other athletes than the currently authenticated one.
        /// </summary>
        public const string Athletes = "https://www.strava.com/api/v3/athletes";
        /// <summary>
        /// Url to the Club endpoint used for other athletes than the currently authenticated one.
        /// </summary>
        public const string Club = "https://www.strava.com/api/v3/clubs";
        /// <summary>
        /// Url to the Club endpoint used for the currently authenticated athlete.
        /// </summary>
        public const string Clubs = "https://www.strava.com/api/v3/athlete/clubs";
        /// <summary>
        /// Url to the endpoint used for receiving the friends of the currentlx authenticated user.
        /// </summary>
        public const string OwnFriends = "https://www.strava.com/api/v3/athlete/friends";
        /// <summary>
        /// Url to the endpoint used for receiving the followers of the currently authenticated athlete.
        /// </summary>
        public const string OwnFollowers = "https://www.strava.com/api/v3/athlete/followers";
        /// <summary>
        /// Url to the endpoint used for receiving the followers of athletes other than the currently authenticated one.
        /// </summary>
        public const string OtherFriends = "https://www.strava.com/api/v3/athletes/{0}/friends";
        /// <summary>
        /// Url to the endpoint used for receiving the followers of athletes other than the currently authenticated one.
        /// </summary>
        public const string OtherFollowers = "https://www.strava.com/api/v3/athletes/{0}/followers";
        /// <summary>
        /// Url to the endpoint used for receiving the followers of athletes other than the currently authenticated one.
        /// </summary>
        public const string MutualFriends = "https://www.strava.com/api/v3/athletes/{0}/both-following";
        /// <summary>
        /// Url to the endpoint used for receiving the K/QOMs/CRs of athletes.
        /// </summary>
        public const string Koms = "https://www.strava.com/api/v3/athletes/{0}/koms";
        /// <summary>
        /// Url to the endpoint used for receiving gear.
        /// </summary>
        public const string Gear = "https://www.strava.com/api/v3/gear";
        /// <summary>
        /// Url to the endpoint used for receiving segment information.
        /// </summary>
        public const string Segment = "https://www.strava.com/api/v3/segments";
        /// <summary>
        /// Url to the endpoint used for receiving segment effort information.
        /// </summary>
        public const string SegmentEffort = "https://www.strava.com/api/v3/segment_efforts";
        /// <summary>
        /// Url to the endpoint used for receiving segment leaderboard information.
        /// </summary>
        public const string Leaderboard = "https://www.strava.com/api/v3/segments/{0}/leaderboard";
        /// <summary>
        /// Url to the endpoint used for receiving starred segments.
        /// </summary>
        public const string Starred = "https://www.strava.com/api/v3/segments/starred";
        /// <summary>
        /// Url to the endpoint used for uploads.
        /// </summary>
        public const string Uploads = "https://www.strava.com/api/v3/uploads/";
    }
}
