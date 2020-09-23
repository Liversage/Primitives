using Liversage.Primitives;
using System;

namespace EntityFramework
{
    [Primitive]
    public readonly partial struct ExternalReference
    {
        readonly Guid category;
    }
}
