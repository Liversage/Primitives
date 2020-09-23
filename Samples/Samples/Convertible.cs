using Liversage.Primitives;
using System;

namespace Samples
{
    [Primitive(Features.Default | Features.Convertible)]
    public readonly partial struct Convertible
    {
        readonly float provider;
    }
}
