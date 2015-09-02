using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// This class represents an athlete.
    /// </summary>
    public class AthleteMeta
    {
        /// <summary>
        /// The Strava athlete id.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// The representation of the Athlete object:
        /// 1 - meta
        /// 2 - summary
        /// 3 - detailed
        /// Listed in increasing levels of detail.
        /// </summary>
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }
    }
}
