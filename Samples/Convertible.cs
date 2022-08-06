using Liversage.Primitives;

namespace Samples;

[Primitive(Features.Default | Features.Convertible)]
public readonly partial struct Convertible
{
    readonly float value;
}
