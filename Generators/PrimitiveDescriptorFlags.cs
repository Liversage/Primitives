using System;

namespace Liversage.Primitives.Generators
{
    [Flags]
    enum PrimitiveDescriptorFlags
    {
        None,
        InnerIsNullable,
        InnerIsString,
        InnerIsComparable = InnerIsString << 1,
        HasConstructor = InnerIsComparable << 1,
        HasToString = HasConstructor << 1
    }
}
