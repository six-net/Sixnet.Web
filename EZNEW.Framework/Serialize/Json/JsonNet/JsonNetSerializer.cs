using System;
using System.Collections.Generic;
using EZNEW.Framework.Serialize;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EZNEW.Framework.Serialize.Json.JsonNet
{
    /// <summary>
    /// Json.Net Serializer
    /// </summary>
    public class JsonNetSerializer : IJsonSerializer
    {
        static readonly JsonSerializePrivatesResolver jsonSerializePrivatesResolver = new JsonSerializePrivatesResolver();
        static readonly BigNumberConverter bigNumberConverter = new BigNumberConverter();
        static readonly PagingConverter pagingConverter = new PagingConverter();

        /// <summary>
        /// serialization an object to a JSON string
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="obj">object</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>json string</returns>
        public string ObjectToJson<T>(T obj, JsonSerializeSetting jsonSerializeSetting)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.GetType().GUID == typeof(string).GUID)
            {
                return obj.ToString();
            }
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(bigNumberConverter);
            if (jsonSerializeSetting != null)
            {
                if (jsonSerializeSetting.ResolveNonPublic)
                {
                    settings.ContractResolver = jsonSerializePrivatesResolver;
                }
            }
            string jsonString = JsonConvert.SerializeObject(obj, settings);
            return jsonString;
        }

        /// <summary>
        /// deserialization a JSON string to an object
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>object</returns>
        public T JsonToObject<T>(string json, JsonSerializeSetting jsonSerializeSetting)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            if (typeof(T).FullName == typeof(string).FullName)
            {
                return (dynamic)json;
            }
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>() { pagingConverter },
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
            if (jsonSerializeSetting != null)
            {
                if (jsonSerializeSetting.ResolveNonPublic)
                {
                    settings.ContractResolver = jsonSerializePrivatesResolver;
                }
            }
            if (jsonSerializeSetting?.DeserializeType == null)
            {
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            else
            {
                var data = JsonConvert.DeserializeObject(json, jsonSerializeSetting.DeserializeType, settings);
                return (T)data;
            }
        }
    }
}
