using Liversage.Primitives;
using System;

namespace EntityFramework
{
    [Primitive]
    public readonly partial struct Timestamp
    {
        readonly DateTimeOffset timestamp;
    }
}
