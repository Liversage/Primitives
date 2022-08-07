using System;

namespace Liversage.Primitives;

[Flags]
enum PrimitiveDescriptorFlags
{
    None,
    InnerIsNullable,
    InnerIsString,
    InnerIsComparable = InnerIsString << 1,
    HasConstructor = InnerIsComparable << 1,
    HasToString = HasConstructor << 1,
    IsInGlobalNamespace = HasToString << 1
}
