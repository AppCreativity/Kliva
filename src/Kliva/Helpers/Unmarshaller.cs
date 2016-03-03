using Newtonsoft.Json;
using System;

namespace Kliva.Helpers
{
    /// <summary>
    /// Converts a Json string to an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Unmarshaller<T>
    {
        /// <summary>
        /// Converts a Json string to an object.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <returns>The converted object of type T.</returns>
        public static T Unmarshal(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                T deserializedObject = JsonConvert.DeserializeObject<T>(json);
                return deserializedObject;
            }

            //TODO: Glenn - do we need to throw Exception?
            // throw new ArgumentException("The json string is null or empty.");

            return default(T);
        }
    }
}
