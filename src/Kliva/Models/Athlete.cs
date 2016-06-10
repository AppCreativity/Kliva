using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// Represents a Strava athlete.
    /// </summary>
    public class Athlete : AthleteSummary
    {
        /// <summary>
        /// The count of the athlete's followers.
        /// </summary>
        [JsonProperty("follower_count")]
        public int FollowerCount { get; set; }

        /// <summary>
        /// The count of the athlete's friends.
        /// </summary>
        [JsonProperty("friend_count")]
        public int FriendCount { get; set; }

        /// <summary>
        /// The count of the athlete's friends that both this athlete and the currently authenticated athlete are following.
        /// </summary>
        [JsonProperty("mutual_friend_count")]
        public int MutualFriendCount { get; set; }

        /// <summary>
        /// The date preference. ISO 8601 time string.
        /// </summary>
        [JsonProperty("date_preference")]
        public string DatePreference { get; set; }

        /// <summary>
        /// Either 'feet' or 'meters'
        /// </summary>
        [JsonProperty("measurement_preference")]
        public string MeasurementPreference { get; set; }

        /// <summary>
        /// The email address.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// The functional threshold power.
        /// </summary>
        [JsonProperty("ftp")]
        public int? Ftp { get; set; }

        /// <summary>
        /// A list of the athlete's bikes.
        /// </summary>
        [JsonProperty("bikes")]
        public List<Bike> Bikes { get; set; }

        /// <summary>
        /// A list of the athlete's shoes.
        /// </summary>
        [JsonProperty("shoes")]
        public List<Shoes> Shoes { get; set; }

        /// <summary>
        /// A list of the athlete's clubs.
        /// </summary>
        [JsonProperty("clubs")]
        public List<Club> Clubs { get; set; }

        /// <summary>
        /// athlete’s default sport type: 0 = cyclist, 1 = runner
        /// </summary>
        [JsonProperty("athlete_type")]
        public int AthleteType { get; set; }
    }
}
