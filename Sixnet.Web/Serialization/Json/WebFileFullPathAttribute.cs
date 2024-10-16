using System;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Sixnet.Web.Serialization.Json.Converter;

namespace Sixnet.Web.Serialization.Json
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class WebFileFullPathAttribute : JsonConverterAttribute
    {
        static readonly ConcurrentDictionary<string, WebFileFullPathJsonConverter> _converters = new();

        public string FileObjectName { get; set; }

        public WebFileFullPathAttribute(string fileObjectName = "")
        {
            FileObjectName = fileObjectName;
        }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            return _converters.GetOrAdd(FileObjectName, k => WebFileFullPathJsonConverter.GetInstance(k));
        }
    }
}
