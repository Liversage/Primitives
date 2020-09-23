using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace Liversage.Primitives.Generators
{
    static class RoslynExtensions
    {
        public static PrimitiveDescriptor? ToPrimitiveDescriptor(this TypeDeclarationSyntax typeDeclaration, SemanticModel semanticModel, IProgress<Diagnostic> progress)
        {
            if (typeDeclaration.Members.OfType<FieldDeclarationSyntax>().Count() != 1)
            {
                progress.Report(Diagnostic.Create(Diagnostics.NotExactlyOneField, typeDeclaration.GetLocation(), typeDeclaration.Identifier.ToString()));
                return null;
            }
            var innerField = typeDeclaration.Members.OfType<FieldDeclarationSyntax>().First().Declaration;

            if (innerField.Variables.Count != 1)
            {
                progress.Report(Diagnostic.Create(Diagnostics.NotExactlyOneField, innerField.GetLocation(), typeDeclaration.Identifier.ToString()));
                return null;
            }

            var innerTypeSymbol = (INamedTypeSymbol) semanticModel.GetSymbolInfo(innerField.Type).Symbol!;
            var innerTypeName = GetInnerTypeName();
            var innerKnownType = GetKnownType();

            var flags = PrimitiveDescriptorFlags.None;
            if (IsInnerNullable())
                flags |= PrimitiveDescriptorFlags.InnerIsNullable;
            if (IsInnerString())
                flags |= PrimitiveDescriptorFlags.InnerIsString;
            if (IsInnerComparable())
                flags |= PrimitiveDescriptorFlags.InnerIsComparable;
            if (HasConstructor())
                flags |= PrimitiveDescriptorFlags.HasConstructor;
            if (HasToString())
                flags |= PrimitiveDescriptorFlags.HasToString;

            if (innerTypeSymbol.IsReferenceType && !flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString))
            {
                progress.Report(Diagnostic.Create(Diagnostics.FieldIsNonStringReference, innerField.GetLocation()));
                return null;
            }

            return new PrimitiveDescriptor(
                typeDeclaration.Identifier.WithoutTrivia(),
                innerField.Variables[0].Identifier,
                innerField.Type,
                innerTypeSymbol,
                innerTypeName,
                innerKnownType,
                flags);

            bool IsInnerNullable() => innerField.Type is NullableTypeSyntax;

            bool IsInnerString() => innerKnownType == KnownType.String;

            bool IsInnerComparable()
            {
                var comparableInterface = innerTypeSymbol.Interfaces
                    .FirstOrDefault(@interface => @interface.OriginalDefinition.ToString() == "System.IComparable<T>");
                return comparableInterface != null;
            }

            bool HasConstructor() => typeDeclaration.Members.OfType<ConstructorDeclarationSyntax>().Any();

            bool HasToString() => typeDeclaration.Members.OfType<MethodDeclarationSyntax>().Any(
                method => method.Identifier.ToString() == nameof(object.ToString)
                    && method.Modifiers.Any(modifier => modifier.Kind() == SyntaxKind.OverrideKeyword));

            string GetInnerTypeName()
            {
                if (innerTypeSymbol.IsTupleType)
                    return "Tuple";
                return innerTypeSymbol.Name + string.Concat(innerTypeSymbol.TypeArguments.Select(t => t.Name));
            }

            KnownType GetKnownType()
            {
                var typeSymbol = innerTypeSymbol;
                if (innerField.Type is NullableTypeSyntax nullableType && semanticModel.GetSymbolInfo(nullableType.ElementType).Symbol is INamedTypeSymbol nonNullableTypeSymbol)
                    typeSymbol = nonNullableTypeSymbol;

                return innerTypeSymbol.SpecialType switch
                {
                    SpecialType.System_SByte => KnownType.SByte,
                    SpecialType.System_Byte => KnownType.Byte,
                    SpecialType.System_Int16 => KnownType.Int16,
                    SpecialType.System_UInt16 => KnownType.UInt16,
                    SpecialType.System_Int32 => KnownType.Int32,
                    SpecialType.System_UInt32 => KnownType.UInt32,
                    SpecialType.System_Int64 => KnownType.Int64,
                    SpecialType.System_UInt64 => KnownType.UInt64,
                    SpecialType.System_Decimal => KnownType.Decimal,
                    SpecialType.System_Single => KnownType.Single,
                    SpecialType.System_Double => KnownType.Double,
                    SpecialType.System_DateTime => KnownType.DateTime,
                    SpecialType.System_String => KnownType.String,
                    _ => (typeSymbol.ToMinimalDisplayString(semanticModel, 0)) switch
                    {
                        "System.DateTimeOffset" => KnownType.DateTimeOffset,
                        "System.TimeSpan" => KnownType.TimeSpan,
                        _ => KnownType.Unknown,
                    },
                };
            }
        }
    }
}
