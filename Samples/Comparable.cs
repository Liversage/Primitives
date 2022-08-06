using Liversage.Primitives;

namespace Samples;

[Primitive(Features.Default | Features.Comparable)]
public readonly partial struct Comparable
{
    readonly int value;

    public static readonly Comparable Zero = new(0);
}
