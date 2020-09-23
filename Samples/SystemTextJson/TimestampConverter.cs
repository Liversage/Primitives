using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJson
{
    public class TimestampConverter : JsonConverter<Timestamp>
    {
        public override Timestamp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Timestamp.FromDateTimeOffset(DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()));

        public override void Write(Utf8JsonWriter writer, Timestamp value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.ToDateTimeOffset().ToUnixTimeMilliseconds());
    }
}
