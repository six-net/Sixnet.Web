using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Web.Mvc
{
    public class BigNumberJsonConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            string[] allowTypeValues = new string[] { typeof(long).FullName, typeof(ulong).FullName, typeof(decimal).FullName };
            return allowTypeValues.Contains(objectType.FullName);
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
