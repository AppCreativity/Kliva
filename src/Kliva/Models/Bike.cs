using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// This class represents gear.
    /// </summary>
    public class Bike : GearSummary
    {
        /// <summary>
        /// The gear's brand name.
        /// </summary>
        [JsonProperty("brand_name")]
        public string Brand { get; set; }

        /// <summary>
        /// The gear's model.
        /// </summary>
        [JsonProperty("model_name")]
        public string Model { get; set; }

        /// <summary>
        /// The type of bike.
        /// </summary>
        [JsonProperty("frame_type")]
        private string _frameType = string.Empty;

        /// <summary>
        /// The type of bike.
        /// </summary>
        public BikeType FrameType
        {
            get
            {
                if (_frameType.Equals("1"))
                {
                    return BikeType.Mountain;
                }
                if (_frameType.Equals("2"))
                {
                    return BikeType.Cross;
                }
                if (_frameType.Equals("3"))
                {
                    return BikeType.Road;
                }
                if (_frameType.Equals("4"))
                {
                    return BikeType.Timetrial;
                }

                return BikeType.Road;
            }
        }

        /// <summary>
        /// The gear's description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }

}
