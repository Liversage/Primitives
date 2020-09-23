using Liversage.Primitives;
using System;
using System.Text.Json.Serialization;

namespace SystemTextJson
{
    [Primitive]
    [JsonConverter(typeof(TimestampConverter))]
    public readonly partial struct Timestamp
    {
        readonly DateTimeOffset timestamp;

        public static Timestamp Now => DateTimeOffset.Now;
        public static Timestamp UtcNow => DateTimeOffset.UtcNow;

        public Timestamp ToLocalTime() => timestamp.ToLocalTime();

        public Timestamp ToUniversalTime() => timestamp.ToUniversalTime();
    }
}
