using EZNEW.Framework.IoC;
using EZNEW.Framework.Serialize.Json.JsonNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Serialize
{
    /// <summary>
    /// json serialize
    /// </summary>
    public static class JsonSerialize
    {
        static readonly IJsonSerializer jsonSerializer = null;
        static JsonSerializeSetting resolveNonPublicSetting = new JsonSerializeSetting() { ResolveNonPublic = true };
        static JsonSerialize()
        {
            jsonSerializer = ContainerManager.Resolve<IJsonSerializer>();
            if (jsonSerializer == null)
            {
                jsonSerializer = new JsonNetSerializer();
            }
        }

        #region DataContractJsonSerializer

        /// <summary>
        /// serialization an object to JSON string by DataContract
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="obj">object</param>
        /// <returns>Json String</returns>
        public static string DataContractObjectToJson<T>(T obj)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream())
            {
                js.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// deserialization a json string to an object
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="jsonValue">json value</param>
        /// <returns>data object</returns>
        public static T JsonToDataContractObject<T>(string jsonValue)
        {
            if (string.IsNullOrEmpty(jsonValue))
            {
                return default(T);
            }
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
            var byteValues = Encoding.UTF8.GetBytes(jsonValue);
            using (MemoryStream stream = new MemoryStream(byteValues))
            {
                return (T)js.ReadObject(stream);
            }
        }

        #endregion

        #region JavaScriptSerializer

        /// <summary>
        /// serialization an object to JSON string by IJsonSerializer
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="obj">data object</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>Json string</returns>
        public static string ObjectToJson<T>(T obj, JsonSerializeSetting jsonSerializeSetting)
        {
            return jsonSerializer.ObjectToJson(obj, jsonSerializeSetting);
        }

        /// <summary>
        /// serialization an object to JSON string by IJsonSerializer
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="obj">data object</param>
        /// <param name="resolveNonPublic">resolveNonPublic</param>
        /// <returns>Json string</returns>
        public static string ObjectToJson<T>(T obj, bool resolveNonPublic = false)
        {
            return ObjectToJson<T>(obj, resolveNonPublic ? resolveNonPublicSetting : null);
        }

        /// <summary>
        /// deserialization a json string to an object by IJsonSerializer
        /// </summary>
        /// <param name="json">json string</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>data object</returns>
        public static T JsonToObject<T>(string json, JsonSerializeSetting jsonSerializeSetting)
        {
            return jsonSerializer.JsonToObject<T>(json, jsonSerializeSetting);
        }

        /// <summary>
        /// deserialization a json string to an object by IJsonSerializer
        /// </summary>
        /// <param name="json">json string</param>
        /// <param name="resolveNonPublic">resolveNonPublic</param>
        /// <returns>data object</returns>
        public static T JsonToObject<T>(string json, bool resolveNonPublic = false)
        {
            return JsonToObject<T>(json, resolveNonPublic ? resolveNonPublicSetting : null);
        }

        #endregion
    }
}
