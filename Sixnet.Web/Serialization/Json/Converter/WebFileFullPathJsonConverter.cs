using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sixnet.Web.Utility;

namespace Sixnet.Web.Serialization.Json.Converter
{
    public sealed class WebFileFullPathJsonConverter : JsonConverter<string>
    {
        readonly string fileObjectName;

        private WebFileFullPathJsonConverter(string fileObjectName)
        {
            this.fileObjectName = fileObjectName;
        }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(HttpClientHelper.GetFullPath(value, fileObjectName));
        }

        public static WebFileFullPathJsonConverter GetInstance(string fileObjectName)
        {
            return new WebFileFullPathJsonConverter(fileObjectName);
        }
    }
}
