using Liversage.Primitives;
using System;

namespace Samples;

[Primitive(Features.Default | Features.Formattable | Features.Parseable)]
public readonly partial struct BasedOnDateTime
{
    readonly DateTime timestamp;

    public static BasedOnDateTime UtcNow => FromDateTime(DateTime.UtcNow);
}
