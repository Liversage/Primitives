using Liversage.Primitives;

namespace Samples;

[Primitive(Features.Default | Features.Formattable | Features.Parseable)]
public readonly partial struct Parseable
{
    readonly int value;
}
