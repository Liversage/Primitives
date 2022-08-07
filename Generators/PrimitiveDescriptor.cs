using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Liversage.Primitives;

record PrimitiveDescriptor(
    Features Features,
    StringComparison StringComparison,
    bool MarkAsNonUserCode,
    string NamespaceName,
    SyntaxToken Name,
    SyntaxToken InnerName,
    TypeSyntax InnerType,
    INamedTypeSymbol InnerTypeSymbol,
    string InnerTypeName,
    KnownType InnerKnownType,
    Location InnerLocation,
    PrimitiveDescriptorFlags Flags);
