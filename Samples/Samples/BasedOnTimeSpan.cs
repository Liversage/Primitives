using Liversage.Primitives;
using System;

namespace Samples
{
    [Primitive(Features.Default | Features.Formattable | Features.Parsable)]
    public readonly partial struct BasedOnTimeSpan
    {
        readonly TimeSpan duration;
    }
}
