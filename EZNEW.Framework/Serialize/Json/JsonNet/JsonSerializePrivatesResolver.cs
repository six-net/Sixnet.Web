using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EZNEW.Framework.Serialize.Json.JsonNet
{
    public class JsonSerializePrivatesResolver : DefaultContractResolver
    {
        static ConcurrentDictionary<Guid, List<JsonProperty>> cacheJsonPropertys = new ConcurrentDictionary<Guid, List<JsonProperty>>();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            if (cacheJsonPropertys.TryGetValue(type.GUID, out List<JsonProperty> jsonPropertys))
            {
                return jsonPropertys ?? new List<JsonProperty>(0);
            }
            jsonPropertys = new List<JsonProperty>();
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(c => !typeof(Delegate).IsAssignableFrom(c.FieldType));
            foreach (var field in fields)
            {
                var jsonProperty = base.CreateProperty(field, MemberSerialization.Fields);
                jsonProperty.Writable = true;
                jsonProperty.Readable = true;
                jsonPropertys.Add(jsonProperty);
            }
            cacheJsonPropertys[type.GUID] = jsonPropertys;
            return jsonPropertys;
        }
    }
}
