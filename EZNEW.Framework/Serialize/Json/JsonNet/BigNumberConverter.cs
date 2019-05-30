using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Serialize.Json.JsonNet
{
    /// <summary>
    /// big number json converter
    /// </summary>
    public class BigNumberConverter : JsonConverter
    {
        static readonly HashSet<Guid> allowTypeValues = new HashSet<Guid> { typeof(long).GUID, typeof(long?).GUID, typeof(ulong).GUID, typeof(ulong?).GUID, typeof(decimal).GUID, typeof(decimal?).GUID };
        public override bool CanConvert(Type objectType)
        {
            return allowTypeValues.Contains(objectType.GUID);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string valueString = value.ToString();
            writer.WriteValue(valueString);
        }
    }
}
