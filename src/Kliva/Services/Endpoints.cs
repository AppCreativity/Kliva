using System;

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
        public const String Activity = "https://www.strava.com/api/v3/activities";
        /// <summary>
        /// Url to the Activity endpoint used for other athletes than the currently authenticated one.
        /// </summary>
        public const String Activities = "https://www.strava.com/api/v3/athlete/activities";
        /// <summary>
        /// Url to the Followers endpoint.
        /// </summary>
        public const String ActivitiesFollowers = "https://www.strava.com/api/v3/activities/following";
        /// <summary>
        /// Url to the Athlete endpoint used for the currently authenticated athlete.
        /// </summary>
        public const String Athlete = "https://www.strava.com/api/v3/athlete";
        /// <summary>
        /// Url to the Athlete endpoint used for other athletes than the currently authenticated one.
        /// </summary>
        public const String Athletes = "https://www.strava.com/api/v3/athletes";
        /// <summary>
        /// Url to the Club endpoint used for other athletes than the currently authenticated one.
        /// </summary>
        public const String Club = "https://www.strava.com/api/v3/clubs";
        /// <summary>
        /// Url to the Club endpoint used for the currently authenticated athlete.
        /// </summary>
        public const String Clubs = "https://www.strava.com/api/v3/athlete/clubs";
        /// <summary>
        /// Url to the endpoint used for receiving the friends of the currentlx authenticated user.
        /// </summary>
        public const String Friends = "https://www.strava.com/api/v3/athlete/friends";
        /// <summary>
        /// Url to the endpoint used for receiving the followers of the currently authenticated athlete.
        /// </summary>
        public const String Follower = "https://www.strava.com/api/v3/athlete/followers";
        /// <summary>
        /// Url to the endpoint used for receiving the followers of athletes other than the currently authenticated one.
        /// </summary>
        public const String Followers = "https://www.strava.com/api/v3/athletes";
        /// <summary>
        /// Url to the endpoint used for receiving gear.
        /// </summary>
        public const String Gear = "https://www.strava.com/api/v3/gear";
        /// <summary>
        /// Url to the endpoint used for receiving segment information.
        /// </summary>
        public const String Leaderboard = "https://www.strava.com/api/v3/segments";
        /// <summary>
        /// Url to the endpoint used for receiving starred segments.
        /// </summary>
        public const String Starred = "https://www.strava.com/api/v3/segments/starred";
        /// <summary>
        /// Url to the endpoint used for uploads.
        /// </summary>
        public const String Uploads = "https://www.strava.com/api/v3/uploads/";
    }
}
