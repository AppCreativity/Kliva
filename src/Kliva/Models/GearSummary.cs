using System;
using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// Represents a piece of gear (Bike or Shoes). This object only contains summary details.
    /// </summary>
    public class GearSummary
    {
        /// <summary>
        /// The gear's id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// True if this is the primary gear.
        /// </summary>
        [JsonProperty("primary")]
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gear's name. Athlete entered for bikes, generated from brand and model for shoes
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Distance travelled with gear in meters.
        /// </summary>
        [JsonProperty("distance")]
        public float Distance { get; set; }

        /// <summary>
        /// Resource state / level of details.
        /// </summary>
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }
    }
}
