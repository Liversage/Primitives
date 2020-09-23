using Liversage.Primitives;
using System.Text.Json.Serialization;

namespace SystemTextJson
{
    [Primitive]
    [JsonConverter(typeof(CategoryConverter))]
    public readonly partial struct Category
    {
        readonly string category;
    }
}
