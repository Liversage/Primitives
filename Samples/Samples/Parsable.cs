using Liversage.Primitives;

namespace Samples
{
    [Primitive(Features.Default | Features.Formattable | Features.Parsable)]
    public readonly partial struct Parsable
    {
        readonly int provider;
    }
}
