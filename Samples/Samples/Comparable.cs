using Liversage.Primitives;

namespace Samples
{
    [Primitive(Features.Default | Features.Comparable)]
    public readonly partial struct Comparable
    {
        readonly int other;

        public static readonly Comparable Zero = new Comparable(0);
    }
}
