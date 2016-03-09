using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kliva.Models.Converters
{
    /// <summary>
    /// This will look at the fields of the athlete and will create an object based on those fields
    /// </summary>
    public class AthleteJsonConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            AthleteMeta athleteMeta;
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            if (FieldExists("athlete_type", jObject))
            {
                athleteMeta = new Athlete();
            }
            else if (FieldExists("firstname", jObject))
            {
                athleteMeta = new AthleteSummary();
            }
            else
            {
                athleteMeta = new AthleteMeta();
            }

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), athleteMeta);

            return athleteMeta;
        }

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(AthleteMeta));
        }
    }
}
