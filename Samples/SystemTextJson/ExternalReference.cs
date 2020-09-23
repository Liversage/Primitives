using Liversage.Primitives;
using System;
using System.Text.Json.Serialization;

namespace SystemTextJson
{
    [Primitive]
    [JsonConverter(typeof(ExternalReferenceConverter))]
    public readonly partial struct ExternalReference
    {
        readonly Guid category;
    }
}
