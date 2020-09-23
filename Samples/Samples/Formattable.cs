using Liversage.Primitives;

namespace Samples
{
    [Primitive(Features.Default | Features.Formattable)]
    public readonly partial struct Formattable
    {
        readonly decimal value;
    }
}
