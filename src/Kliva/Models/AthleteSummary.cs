using System;
using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// Athletes are Strava users, Strava users are athletes. This is a less detailed version of an athlete.
    /// </summary>
    public partial class AthleteSummary : AthleteMeta
    {
        /// <summary>
        /// The athletes first name.
        /// </summary>
        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        /// <summary>
        /// The athletes last name.
        /// </summary>
        [JsonProperty("lastname")]
        public string LastName { get; set; }

        /// <summary>
        /// Url to a 62x62 pixel profile picture. You can use the ImageLoader class to load this picture.
        /// </summary>
        [JsonProperty("profile_medium")]
        public string ProfileMedium { get; set; }

        /// <summary>
        /// Url to a 124x124 pixel profile picture. You can use the ImageLoader class to load this picture.
        /// </summary>
        [JsonProperty("profile")]
        public string Profile { get; set; }

        /// <summary>
        /// The city where the athlete lives.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// The state where the athlete lives.
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// The state where the athlete lives.
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// The athlete's sex.
        /// </summary>
        [JsonProperty("sex")]
        public string Sex { get; set; }

        /// <summary>
        /// The authenticated athlete’s friend status of this athlete.
        /// Values are 'pending', 'accepted', 'blocked' or 'null'.
        /// </summary>
        [JsonProperty("friend")]
        public string Friend { get; set; }

        /// <summary>
        /// The authenticated athlete’s following status of this athlete.
        /// Values are 'pending', 'accepted', 'blocked' or 'null'.
        /// </summary>
        [JsonProperty("follower")]
        public string Follower { get; set; }

        /// <summary>
        /// True, if the athlete is a Strava premium member. In some cases this attribute is important, for example when leaderboards are filtered
        /// by either weight class or age group.
        /// </summary>
        [JsonProperty("premium")]
        public bool IsPremium { get; set; }

        /// <summary>
        /// The date when this athlete was created. ISO 8601 time string.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date when this athlete was updated. ISO 8601 time string.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// True, if enhanced privacy is enabled.
        /// </summary>
        [JsonProperty("approve_followers")]
        public bool ApproveFollowers { get; set; }
    }

    /// <summary>
    /// Seperated added fields from original response class!
    /// </summary>
    public partial class AthleteSummary
    {
        public string ProfileMediumFormatted
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfileMedium) && ProfileMedium.Contains("http"))
                    return ProfileMedium;
                else
                    return Constants.STRAVA_DEFAULT_AVATAR;
            }
        }

        public string ProfileLargeFormatted
        {
            get
            {
                if (!string.IsNullOrEmpty(Profile) && Profile.Contains("http"))
                    return Profile;
                else
                    return Constants.STRAVA_DEFAULT_AVATAR;
            }
        }

        public string FullName => string.Concat(FirstName, " ", LastName);

        public string CityFormatted => City + ", " + State + ", " + Country;
    }
}
