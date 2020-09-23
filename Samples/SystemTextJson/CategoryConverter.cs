using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJson
{
    public class CategoryConverter : JsonConverter<Category>
    {
        public override Category Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Category.FromString(reader.GetString());

        public override void Write(Utf8JsonWriter writer, Category value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
