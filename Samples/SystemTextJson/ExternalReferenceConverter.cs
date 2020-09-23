using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJson
{
    public class ExternalReferenceConverter : JsonConverter<ExternalReference>
    {
        public override ExternalReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => ExternalReference.FromGuid(reader.GetGuid());

        public override void Write(Utf8JsonWriter writer, ExternalReference value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
