using Liversage.Primitives;

namespace Samples
{
    [Primitive]
    public readonly partial struct Tuple
    {
        readonly (Direction Direction, int Distance) value;
    }
}
