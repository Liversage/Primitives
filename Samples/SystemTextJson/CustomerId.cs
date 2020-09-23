using Liversage.Primitives;
using System.Text.Json.Serialization;

namespace SystemTextJson
{
    [Primitive]
    [JsonConverter(typeof(CustomerIdConverter))]
    public readonly partial struct CustomerId
    {
        readonly int id;
    }
}
