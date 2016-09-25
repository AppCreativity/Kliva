using System;
using Kliva.Helpers;
using Newtonsoft.Json;
using Kliva.Models.Interfaces;

namespace Kliva.Models
{
    /// <summary>
    /// Represents a less detailed version of a segment.
    /// </summary>
    public partial class SegmentSummary
    {
        /// <summary>
        /// The id provided by Strava.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The resource state.
        /// </summary>
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        /// <summary>
        /// The name of the segment.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of activity.
        /// </summary>
        [JsonProperty("activity_type")]
        public string ActivityType { get; set; }

        /// <summary>
        /// The segment's distance.
        /// </summary>
        [JsonProperty("distance")]
        public float Distance { get; set; }

        /// <summary>
        /// The average grade of the segment.
        /// </summary>
        [JsonProperty("average_grade")]
        public float AverageGrade { get; set; }

        /// <summary>
        /// The max grade of the segment.
        /// </summary>
        [JsonProperty("maximum_grade")]
        public float MaxGrade { get; set; }

        /// <summary>
        /// The segment's highest elevation.
        /// </summary>
        [JsonProperty("elevation_high")]
        public float MaxElevation { get; set; }

        /// <summary>
        /// The segment's lowest elevation.
        /// </summary>
        [JsonProperty("elevation_low")]
        public float MinElevation { get; set; }

        /// <summary>
        /// the climb category of the segment.
        /// </summary>
        [JsonProperty("climb_category")]
        public int Category { get; set; }

        public ClimbCategory ClimbCategory
        {
            get
            {
                switch (Category)
                {
                    case 0:
                        return ClimbCategory.CategoryHc;
                    case 1:
                        return ClimbCategory.Category4;
                    case 2:
                        return ClimbCategory.Category3;
                    case 3:
                        return ClimbCategory.Category2;
                    case 4:
                        return ClimbCategory.Category1;
                    case 5:
                        return ClimbCategory.CategoryNc;
                    default:
                        return ClimbCategory.CategoryNc;
                }
            }
        }

        /// <summary>
        /// The city where this segment is located in.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// The state where this segment is located in.
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// The country where this segment is located in.
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// True if this segment is private.
        /// </summary>
        [JsonProperty("private")]
        public Boolean IsPrivate { get; set; }

        /// <summary>
        /// True if the segment is starred by the currently authenticated athlete.
        /// </summary>
        [JsonProperty("starred")]
        public Boolean IsStarred { get; set; }
    }

    /// <summary>
    /// Separated added fields from original response class!
    /// </summary>
    public partial class SegmentSummary : BaseClass, ISupportUserMeasurementUnits
    {
        public DistanceUnitType MeasurementUnit
        {
            get;
            private set;
        }

        public void SetUserMeasurementUnits(DistanceUnitType measurementUnit)
        {
            MeasurementUnit = measurementUnit;

            DistanceUserMeasurementUnit = new UserMeasurementUnitMetric(Distance, DistanceUnitType.Metres, MeasurementUnit);
        }

        public UserMeasurementUnitMetric DistanceUserMeasurementUnit
        {
            get;
            private set;
        }
    }
}
