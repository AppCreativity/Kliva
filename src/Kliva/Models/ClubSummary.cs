using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// A summary of a Strava Club. The ClubSummary contains less information than a Club.
    /// </summary>
    public partial class ClubSummary : BaseClass
    {
        /// <summary>
        /// The id of the club. The id is provided by Strava and can't be changed.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// The level of detail of the Club.
        /// </summary>
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        /// <summary>
        /// The name of the club.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Url to a 62x62 pixel picture of the club. Use the ImageLoader class to load the picture.
        /// </summary>
        [JsonProperty("profile_medium")]
        public string ProfileMedium { get; set; }

        /// <summary>
        /// Url to a 124x124 pixel picture of the club. Use the ImageLoader class to load the picture.
        /// </summary>
        [JsonProperty("profile")]
        public string Profile { get; set; }
    }

    /// <summary>
    /// Separated added fields from original response class!
    /// </summary>
    public partial class ClubSummary
    {
        private List<AthleteSummary> _members;
        public List<AthleteSummary> Members
        {
            get { return _members; }
            set { Set(() => Members, ref _members, value); }
        }
    }
}
