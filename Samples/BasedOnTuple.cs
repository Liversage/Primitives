using Liversage.Primitives;

namespace Samples;

[Primitive]
public readonly partial struct BasedOnTuple
{
    readonly (Direction Direction, int Distance) value;
}
