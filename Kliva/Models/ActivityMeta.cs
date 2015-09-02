using Newtonsoft.Json;

namespace Kliva.Models
{
    public class ActivityMeta : BaseClass
    {
        /// <summary>
        /// The id of the activity. This id is provided by Strava at upload.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
