using EZNEW.Framework.Paging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Framework.Serialize.Json.JsonNet
{
    public class PagingConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (!objectType.IsGenericType)
            {
                return false;
            }
            var pagingType = typeof(IPaging<>);
            var genericType = objectType.GetGenericTypeDefinition();
            if (genericType == null)
            {
                return false;
            }
            return genericType == pagingType || pagingType.IsAssignableFrom(genericType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type genericType = objectType.GetGenericArguments()[0];
            Type pagingType = typeof(Paging<>).MakeGenericType(genericType);
            return serializer.Deserialize(reader, pagingType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
