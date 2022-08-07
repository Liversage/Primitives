using Microsoft.CodeAnalysis.CSharp;
using System;

namespace Liversage.Primitives;

static class KnownTypeExtensions
{
    public static SyntaxKind ToTypeKeyword(this KnownType knownType) => knownType switch
    {
        KnownType.SByte => SyntaxKind.SByteKeyword,
        KnownType.Byte => SyntaxKind.ByteKeyword,
        KnownType.Int16 => SyntaxKind.ShortKeyword,
        KnownType.UInt16 => SyntaxKind.UShortKeyword,
        KnownType.Int32 => SyntaxKind.IntKeyword,
        KnownType.UInt32 => SyntaxKind.UIntKeyword,
        KnownType.Int64 => SyntaxKind.LongKeyword,
        KnownType.UInt64 => SyntaxKind.ULongKeyword,
        KnownType.Decimal => SyntaxKind.DecimalKeyword,
        KnownType.Single => SyntaxKind.FloatKeyword,
        KnownType.Double => SyntaxKind.DoubleKeyword,
        KnownType.String => SyntaxKind.StringKeyword,
        _ => throw new ArgumentException("Known type has no equivalent type keyword.", nameof(knownType))
    };
}
