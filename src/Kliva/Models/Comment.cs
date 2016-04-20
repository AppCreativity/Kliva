using System;
using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// Represents a comment of an activity.
    /// </summary>
    public partial class Comment
    {
        /// <summary>
        /// The comment id.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// The resource state.
        /// </summary>
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        /// <summary>
        /// The activity to which the comment was posted.
        /// </summary>
        [JsonProperty("activity_id")]
        public long ActivityId { get; set; }

        /// <summary>
        /// The comment's text.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// The athlete who wrote the comment.
        /// </summary>
        [JsonProperty("athlete")]
        public AthleteSummary Athlete { get; set; }

        /// <summary>
        /// The time when the comment was crated.
        /// </summary>
        [JsonProperty("created_at")]
        public string TimeCreated { get; set; }
    }

    public partial class Comment
    {
        /// <summary>
        /// Returns the TimeCreated as a DateTime object.
        /// </summary>
        public DateTime DateTimeCreated => DateTime.Parse(TimeCreated);
    }
}
