using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Framework.Serialize
{
    /// <summary>
    /// JSON Serializer
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// serialization an object to a JSON string
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="obj">object</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>json string</returns>
        string ObjectToJson<T>(T obj, JsonSerializeSetting jsonSerializeSetting);

        /// <summary>
        /// deserialization a JSON string to an object
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>object</returns>
        T JsonToObject<T>(string json, JsonSerializeSetting jsonSerializeSetting);
    }
}
