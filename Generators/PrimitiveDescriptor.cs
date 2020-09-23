using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Liversage.Primitives.Generators
{
    class PrimitiveDescriptor
    {
        public PrimitiveDescriptor(
            SyntaxToken name, SyntaxToken innerName, TypeSyntax innerType, INamedTypeSymbol innerTypeSymbol,
            string innerTypeName, KnownType innerKnownType, PrimitiveDescriptorFlags flags)
        {
            Name = name;
            InnerName = innerName;
            InnerType = innerType ?? throw new ArgumentNullException(nameof(innerType));
            InnerTypeSymbol = innerTypeSymbol ?? throw new ArgumentNullException(nameof(innerTypeSymbol));
            InnerTypeName = innerTypeName ?? throw new ArgumentNullException(nameof(innerTypeName));
            InnerKnownType = innerKnownType;
            Flags = flags;
        }

        public SyntaxToken Name { get; }
        public SyntaxToken InnerName { get; }
        public TypeSyntax InnerType { get; }
        public INamedTypeSymbol InnerTypeSymbol { get; }
        public string InnerTypeName { get; }
        public KnownType InnerKnownType { get; }
        public PrimitiveDescriptorFlags Flags { get; }
    }
}
