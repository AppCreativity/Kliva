using System;
using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// Represents a Strava segment.
    /// </summary>
    public partial class Segment : SegmentSummary
    {
        /// <summary>
        /// The date when the segment was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// The date when the segment was updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        /// <summary>
        /// The total elevation gain of the segment.
        /// </summary>
        [JsonProperty("total_elevation_gain")]
        public float TotalElevationGain { get; set; }

        /// <summary>
        /// A map with the segment's route.
        /// </summary>
        [JsonProperty("map")]
        public Map Map { get; set; }

        /// <summary>
        /// The effort count.
        /// </summary>
        [JsonProperty("effort_count")]
        public int EffortCount { get; set; }

        /// <summary>
        /// The number of athletes that run or rode this segment.
        /// </summary>
        [JsonProperty("athlete_count")]
        public int AthleteCount { get; set; }

        /// <summary>
        /// True if the segment was marked as hazardous.
        /// </summary>
        [JsonProperty("hazardous")]
        public Boolean IsHazardous { get; set; }

        /// <summary>
        /// The personal record time.
        /// </summary>
        [JsonProperty("pr_time")]
        public int PersonalRecordTime { get; set; }

        /// <summary>
        /// The personal record distance.
        /// </summary>
        [JsonProperty("pr_distance")]
        public float PersonalRecordDistance { get; set; }

        /// <summary>
        /// Number of stars on this segment. 
        /// </summary>
        [JsonProperty("star_count")]
        public int StarCount { get; set; }
    }
}
