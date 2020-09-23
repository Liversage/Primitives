using Liversage.Primitives;
using System;

namespace Samples
{
    [Primitive]
    public readonly partial struct BasedOnGuid
    {
        readonly Guid id;

        public static BasedOnGuid CreateRandom() => Guid.NewGuid();
    }
}
