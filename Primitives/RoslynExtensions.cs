using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace Liversage.Primitives;

static class RoslynExtensions
{
    public static PrimitiveDescriptor? ToPrimitiveDescriptor(this StructDeclarationSyntax @struct, SemanticModel semanticModel, string attributeName,
        IProgress<Diagnostic> progress)
    {
        var symbol = semanticModel.GetDeclaredSymbol(@struct);
        var attributes = symbol?.GetAttributes();
        var attribute = attributes?.FirstOrDefault(data => data.AttributeClass?.ToString() == attributeName);
        if (attribute is null)
            return null;
        var features = GetFeatures();
        if (!features.HasValue)
            return null;
        var stringComparison = GetStringComparison();
        if (!stringComparison.HasValue)
            return null;
        var markAsNonUserCode = GetMarkAsNonUserCode();
        if (!markAsNonUserCode.HasValue)
            return null;

        if (@struct.Members.OfType<FieldDeclarationSyntax>().Count(FieldPredicate) != 1)
        {
            progress.Report(
                Diagnostic.Create(Diagnostics.NotExactlyOneField, @struct.GetLocation(), @struct.Identifier.ToString()));
            return null;
        }
        var innerField = @struct.Members.OfType<FieldDeclarationSyntax>().First(FieldPredicate).Declaration;

        if (innerField.Variables.Count != 1)
        {
            progress.Report(
                Diagnostic.Create(Diagnostics.NotExactlyOneField, innerField.GetLocation(), @struct.Identifier.ToString()));
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
        if (IsInGlobalNamespace())
            flags |= PrimitiveDescriptorFlags.IsInGlobalNamespace;

        if (innerTypeSymbol.IsReferenceType && !flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString))
        {
            progress.Report(Diagnostic.Create(Diagnostics.FieldIsNonStringReference, innerField.GetLocation()));
            return null;
        }

        return new PrimitiveDescriptor(
            features.Value,
            stringComparison.Value,
            markAsNonUserCode.Value,
            symbol!.ContainingNamespace.ToDisplayString(),
            @struct.Identifier.WithoutTrivia(),
            innerField.Variables[0].Identifier,
            innerField.Type,
            innerTypeSymbol,
            innerTypeName,
            innerKnownType,
            innerField.GetLocation(),
            flags);

        Features? GetFeatures()
        {
            if (attribute.ConstructorArguments.Length is not 1)
            {
                progress.Report(Diagnostic.Create(Diagnostics.InvalidAttributeConstructor, @struct.GetLocation(), @struct.Identifier.ToString()));
                return null;
            }
            if (attribute.ConstructorArguments[0].Value is not int value)
            {
                progress.Report(Diagnostic.Create(Diagnostics.InvalidAttributeConstructor, @struct.GetLocation(), @struct.Identifier.ToString()));
                return null;
            }
            return (Features) value;
        }

        StringComparison? GetStringComparison() =>
            (StringComparison?) GetNamedArgument("StringComparison", (int) StringComparison.Ordinal);

        bool? GetMarkAsNonUserCode() =>
            GetNamedArgument("MarkAsNonUserCode", true);

        T? GetNamedArgument<T>(string name, T defaultValue) where T : struct
        {
            var argument = attribute.NamedArguments.FirstOrDefault(kvp => kvp.Key == name);
            if (argument.Key is null)
                return defaultValue;
            if (argument.Value.Value is T value)
                return value;
            progress.Report(Diagnostic.Create(Diagnostics.InvalidAttributeConstructor, @struct.GetLocation(), @struct.Identifier.ToString()));
            return null;
        }

        static bool FieldPredicate(FieldDeclarationSyntax field) =>
            !field.Modifiers.Any(SyntaxKind.StaticKeyword) && !field.Modifiers.Any(SyntaxKind.ConstKeyword);

        string GetInnerTypeName() =>
            innerTypeSymbol.IsTupleType ? "Tuple" : innerTypeSymbol.Name + string.Concat(innerTypeSymbol.TypeArguments.Select(t => t.Name));

        KnownType GetKnownType()
        {
            var typeSymbol = innerTypeSymbol;
            if (innerField.Type is NullableTypeSyntax nullableType &&
                semanticModel.GetSymbolInfo(nullableType.ElementType).Symbol is INamedTypeSymbol nonNullableTypeSymbol)
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
                _ => typeSymbol.ToMinimalDisplayString(semanticModel, 0) switch
                {
                    "System.DateTimeOffset" => KnownType.DateTimeOffset,
                    "System.TimeSpan" => KnownType.TimeSpan,
                    _ => KnownType.Unknown
                }
            };
        }

        bool IsInnerNullable() => innerField.Type is NullableTypeSyntax;

        bool IsInnerString() => innerKnownType == KnownType.String;

        bool IsInnerComparable()
        {
            var comparableInterface = innerTypeSymbol.Interfaces
                .FirstOrDefault(@interface => @interface.OriginalDefinition.ToString() == "System.IComparable<T>");
            return comparableInterface != null;
        }

        bool HasConstructor() => @struct.Members.OfType<ConstructorDeclarationSyntax>().Any();

        bool HasToString() => @struct.Members.OfType<MethodDeclarationSyntax>().Any(
            method => method.Identifier.ToString() == nameof(ToString)
                      && method.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.OverrideKeyword)));

        bool IsInGlobalNamespace() => symbol!.ContainingNamespace.IsGlobalNamespace;
    }
}
