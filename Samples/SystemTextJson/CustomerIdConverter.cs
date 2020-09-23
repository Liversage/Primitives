using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJson
{
    public class CustomerIdConverter : JsonConverter<CustomerId>
    {
        public override CustomerId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => CustomerId.FromInt32(reader.GetInt32());

        public override void Write(Utf8JsonWriter writer, CustomerId value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.ToInt32());
    }
}
