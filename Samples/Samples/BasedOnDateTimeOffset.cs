using Liversage.Primitives;
using System;

namespace Samples
{
    [Primitive]
    public readonly partial struct BasedOnDateTimeOffset
    {
        readonly DateTimeOffset timestamp;
    }
}
