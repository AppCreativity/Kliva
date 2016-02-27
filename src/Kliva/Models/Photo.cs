
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kliva.Models
{
    /// <summary>
    /// Represents a photo linked to an activity.
    /// </summary>
    public partial class Photo
    {
        /// <summary>
        /// The photo's id.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// The id of the activity to which the photo is connected to.
        /// </summary>
        [JsonProperty("activity_id")]
        public long ActivityId { get; set; }

        /// <summary>
        /// The level of detail.
        /// </summary>
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        /// <summary>
        /// Url to the picture. Use the ImageLoader class to download the picture.
        /// </summary>
        [JsonProperty("ref")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// The photo's external id.
        /// </summary>
        [JsonProperty("uid")]
        public string ExternalUid { get; set; }

        /// <summary>
        /// The caption.
        /// </summary>
        [JsonProperty("caption")]
        public string Caption { get; set; }

        /// <summary>
        /// Image source. Currently only "InstagramPhoto"
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The date when the image was uploaded.
        /// </summary>
        [JsonProperty("uploaded_at")]
        public string UploadedAt { get; set; }

        /// <summary>
        /// The date when the image was crated.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// The location where the picture was taken.
        /// </summary>
        [JsonProperty("location")]
        public List<double> Location { get; set; }
    }

    /// <summary>
    /// Seperated added fields from original response class!
    /// </summary>
    public partial class Photo
    {
        public string ImageThumbnail => !string.IsNullOrEmpty(ImageUrl) ? $"{ImageUrl}media?size=t" : null;

        public string ImageLarge => !string.IsNullOrEmpty(ImageUrl) ? $"{ImageUrl}media?size=l" : null;
    }
}
