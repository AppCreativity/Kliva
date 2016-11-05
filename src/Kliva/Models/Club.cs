using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// This class represents a Strava Club.
    /// </summary>
    public class Club : ClubSummary
    {
        /// <summary>
        /// The club's description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// The club's type.
        /// </summary>
        [JsonProperty("club_type")]
        private string _clubType { get; set; }

        /// <summary>
        /// The club's type.
        /// </summary>
        public ClubType ClubType
        {
            get
            {
                if (_clubType.Equals("casual_club"))
                    return ClubType.Casual;
                if (_clubType.Equals("racing_team"))
                    return ClubType.RacingTeam;
                if (_clubType.Equals("shop"))
                    return ClubType.Shop;
                if (_clubType.Equals("company"))
                    return ClubType.Company;

                return ClubType.Other;
            }
        }

        /// <summary>
        /// The sports type of the club.
        /// </summary>
        [JsonProperty("sport_type")]
        private string _sportType { get; set; }

        /// <summary>
        /// The sports type of the club.
        /// </summary>
        public SportType SportType
        {
            get
            {
                if (_sportType.Equals("cycling"))
                    return SportType.Cycling;
                if (_sportType.Equals("running"))
                    return SportType.Running;
                if (_sportType.Equals("triathlon"))
                    return SportType.Triathlon;

                return SportType.Other;
            }
        }

        /// <summary>
        /// the club's city.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// The club's state.
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// The club's country.
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// True if the club is a private club.
        /// </summary>
        [JsonProperty("private")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// The club's member count.
        /// </summary>
        [JsonProperty("member_count")]
        public int MemberCount { get; set; }
    }
}
