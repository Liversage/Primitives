using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Liversage.Primitives.Generators
{
    public class PrimitiveGenerator : IRichCodeGenerator
    {
        readonly StringComparison stringComparison;
        Features features;

        public PrimitiveGenerator(AttributeData attributeData)
        {
            if (attributeData is null)
                throw new ArgumentNullException(nameof(attributeData));

            features = GetFeatures();
            stringComparison = (StringComparison?) GetNamedArgument<int>(nameof(PrimitiveAttribute.StringComparison)) ?? StringComparison.Ordinal;

            Features GetFeatures()
            {
                if (attributeData.ConstructorArguments.Length != 1)
                    throw new ArgumentException($"Invalid attribute: Attribute has {attributeData.ConstructorArguments.Length} arguments instead of 1.", nameof(attributeData));
                if (!(attributeData.ConstructorArguments[0].Value is int features))
                    throw new ArgumentException($"Invalid attribute: Argument has wrong type {attributeData.ConstructorArguments[0].Value?.GetType().FullName ?? "null"} with value '{attributeData.ConstructorArguments[0].Value}'.", nameof(attributeData));
                return (Features) features;
            }

            T? GetNamedArgument<T>(string name) where T : struct
            {
                var argument = attributeData.NamedArguments.FirstOrDefault(kvp => kvp.Key == name);
                if (argument.Key == default)
                    return default;
                if (!(argument.Value.Value is T value))
                    throw new ArgumentException($"Invalid attribute: {name} has wrong type {argument.Value.Value?.GetType().FullName ?? "null"} with value '{argument.Value.Value}'.", nameof(attributeData));
                return value;
            }
        }

        public async Task<RichGenerationResult> GenerateRichAsync(
            TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));
            if (progress is null)
                throw new ArgumentNullException(nameof(progress));

            var generatedMembers = await GenerateAsync(context, progress, cancellationToken).ConfigureAwait(false);
            var wrappedMembers = context.ProcessingNode.Ancestors().Aggregate(generatedMembers, WrapInAncestor);
            var result = new RichGenerationResult { Members = wrappedMembers };
            var usings = new List<UsingDirectiveSyntax>();
            if (context.CompilationUnitUsings.All(syntax => syntax.Name.ToString() != "System"))
                usings.Add(UsingDirective(ParseName("System")));
            if (features.HasFlag(Features.Parsable) && context.CompilationUnitUsings.All(syntax => syntax.Name.ToString() != "System.Globalization"))
                usings.Add(UsingDirective(ParseName("System.Globalization")));
            if (usings.Count > 0)
                result.Usings = new SyntaxList<UsingDirectiveSyntax>(usings);
            return result;
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(
            TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));
            if (progress is null)
                throw new ArgumentNullException(nameof(progress));

            var location = context.ProcessingNode.GetLocation();

            if (!(context.ProcessingNode is TypeDeclarationSyntax typeDeclaration &&
                  typeDeclaration.IsKind(SyntaxKind.StructDeclaration)))
            {
                progress.Report(Diagnostic.Create(Diagnostics.NotAStruct, location));
                return Task.FromResult(List<MemberDeclarationSyntax>());
            }

            var descriptor = typeDeclaration.ToPrimitiveDescriptor(context.SemanticModel, progress);
            if (descriptor == null)
                return Task.FromResult(List<MemberDeclarationSyntax>());

            if (features.HasFlag(Features.Comparable) && !descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsComparable))
            {
                progress.Report(Diagnostic.Create(Diagnostics.FieldIsNotComparable, location));
                features &= ~Features.Comparable;
            }

            if (features.HasFlag(Features.Parsable) && (descriptor.InnerKnownType == KnownType.Unknown || descriptor.InnerKnownType == KnownType.String))
            {
                progress.Report(Diagnostic.Create(Diagnostics.FieldIsNotParsable, location));
                features &= ~Features.Parsable;
            }

            return Task.FromResult(SingletonList((MemberDeclarationSyntax) StructSyntax(descriptor)));
        }

        StructDeclarationSyntax StructSyntax(PrimitiveDescriptor descriptor)
        {
            var @struct = StructDeclaration(descriptor.Name)
                .WithModifiers(
                        TokenList(new[] { Token(SyntaxKind.ReadOnlyKeyword), Token(SyntaxKind.PartialKeyword) }));

            var baseTypes = GetBaseTypes(descriptor).ToList();
            if (baseTypes.Count >= 1)
                @struct = @struct
                    .WithBaseList(
                        BaseList(
                            SeparatedList(
                                baseTypes,
                                Enumerable.Repeat(Token(SyntaxKind.CommaToken), baseTypes.Count - 1))));

            @struct = @struct
                .WithMembers(List(GetMembers(descriptor)));

            if (descriptor.IsInnerTypeNullableReferenceType())
                @struct = @struct
                    .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)));

            return @struct;
        }

        IEnumerable<BaseTypeSyntax> GetBaseTypes(PrimitiveDescriptor descriptor)
        {
            if (features.HasFlag(Features.Equatable))
                yield return SimpleBaseType(
                    GenericName(Identifier("IEquatable"))
                        .WithTypeArgumentList(
                            TypeArgumentList(
                                SingletonSeparatedList<TypeSyntax>(
                                    IdentifierName(descriptor.Name)))));

            if (features.HasFlag(Features.Comparable))
            {
                yield return SimpleBaseType(
                    GenericName(Identifier("IComparable"))
                        .WithTypeArgumentList(
                            TypeArgumentList(
                                SingletonSeparatedList<TypeSyntax>(
                                    IdentifierName(descriptor.Name)))));
                yield return SimpleBaseType(IdentifierName("IComparable"));
            }

            if (features.HasFlag(Features.Formattable))
                yield return SimpleBaseType(IdentifierName("IFormattable"));

            if (features.HasFlag(Features.Convertible))
                yield return SimpleBaseType(IdentifierName("IConvertible"));
        }

        [SuppressMessage(
            "Microsoft.Maintainability", "CA1502:Avoid excessive complexity",
            Justification = "Using one big iterator block to drive the generation simplifies the code.")]
        IEnumerable<MemberDeclarationSyntax> GetMembers(PrimitiveDescriptor descriptor)
        {
            if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.HasConstructor))
                yield return descriptor.ConstructorSyntax();

            yield return descriptor.FromPrimitiveSyntax();
            yield return descriptor.ImplictCastFromPrimitiveSyntax();
            if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString))
            {
                yield return descriptor.ToPrimitiveSyntax();
                yield return descriptor.ExplicitCastToPrimitiveSyntax();
            }
            else
            {
                yield return descriptor.StringExplicitCastToPrimitiveSyntax();
            }

            if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.HasToString))
            {
                if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString))
                    yield return descriptor.ToStringSyntax();
                else
                    yield return descriptor.StringToStringSyntax();
            }

            if (features.HasFlag(Features.Equatable))
            {
                if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString))
                    yield return descriptor.IEquatableTEqualsSyntax();
                else
                    yield return descriptor.StringIEquatableTEqualsSyntax(stringComparison);
                yield return descriptor.EqualsSyntax();
                if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString))
                    yield return descriptor.GetHashCodeSyntax();
                else
                    yield return descriptor.StringGetHashCodeSyntax(stringComparison);
                yield return descriptor.OperatorEqualsSyntax();
                yield return descriptor.OperatorNotEqualsSyntax();
            }

            if (features.HasFlag(Features.Comparable))
            {
                if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString))
                    yield return descriptor.IComparableTCompareToSyntax();
                else
                    yield return descriptor.StringIComparableTCompareToSyntax(stringComparison);
                yield return descriptor.IComparableCompareToSyntax();
                yield return descriptor.OperatorLessThanSyntax();
                yield return descriptor.OperatorLessThanOrEqualSyntax();
                yield return descriptor.OperatorGreaterThanSyntax();
                yield return descriptor.OperatorGreaterThanOrEqualSyntax();
            }

            if (features.HasFlag(Features.Formattable))
                yield return descriptor.IFormattableToStringSyntax();

            if (features.HasFlag(Features.Parsable))
            {
                switch (descriptor.InnerKnownType)
                {
                    case KnownType.SByte:
                    case KnownType.Byte:
                    case KnownType.Int16:
                    case KnownType.UInt16:
                    case KnownType.Int32:
                    case KnownType.UInt32:
                    case KnownType.Int64:
                    case KnownType.UInt64:
                    case KnownType.Decimal:
                    case KnownType.Single:
                    case KnownType.Double:
                        yield return descriptor.NumberTryParseStringSyntax();
                        yield return descriptor.NumberTryParseSpanSyntax();
                        break;
                    case KnownType.DateTime:
                    case KnownType.DateTimeOffset:
                        yield return descriptor.DateTimeTryParseStringSyntax();
                        yield return descriptor.DateTimeTryParseSpanSyntax();
                        break;
                    case KnownType.TimeSpan:
                        yield return descriptor.TimeSpanTryParseStringSyntax();
                        yield return descriptor.TimeSpanTryParseSpanSyntax();
                        break;
                }
            }

            if (features.HasFlag(Features.Convertible))
            {
                yield return descriptor.IConvertibleGetTypeCodeSyntax();
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.BoolKeyword, "ToBoolean");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.ByteKeyword, "ToByte");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.CharKeyword, "ToChar");
                yield return descriptor.IConvertibleConvertToSyntax("DateTime", "ToDateTime");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.DecimalKeyword, "ToDecimal");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.DoubleKeyword, "ToDouble");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.ShortKeyword, "ToInt16");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.IntKeyword, "ToInt32");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.LongKeyword, "ToInt64");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.SByteKeyword, "ToSByte");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.FloatKeyword, "ToSingle");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.StringKeyword, "ToString");
                yield return descriptor.IConvertibleToTypeSyntax();
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.UShortKeyword, "ToUInt16");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.UIntKeyword, "ToUInt32");
                yield return descriptor.IConvertibleConvertToSyntax(SyntaxKind.ULongKeyword, "ToUInt64");
            }
        }

        static SyntaxList<MemberDeclarationSyntax> WrapInAncestor(SyntaxList<MemberDeclarationSyntax> generatedMembers, SyntaxNode ancestor)
            => ancestor switch
            {
                NamespaceDeclarationSyntax ancestorNamespace => SingletonList<MemberDeclarationSyntax>(
                    CopyAsAncestor(ancestorNamespace).WithMembers(generatedMembers)),
                ClassDeclarationSyntax nestingClass => SingletonList<MemberDeclarationSyntax>(
                    CopyAsAncestor(nestingClass).WithMembers(generatedMembers)),
                StructDeclarationSyntax nestingStruct => SingletonList<MemberDeclarationSyntax>(
                    CopyAsAncestor(nestingStruct).WithMembers(generatedMembers)),
                _ => generatedMembers
            };

        static NamespaceDeclarationSyntax CopyAsAncestor(NamespaceDeclarationSyntax syntax)
            => NamespaceDeclaration(syntax.Name.WithoutTrivia())
                .WithExterns(List(syntax.Externs.Select(x => x.WithoutTrivia())))
                .WithUsings(List(syntax.Usings.Select(x => x.WithoutTrivia())));

        static ClassDeclarationSyntax CopyAsAncestor(ClassDeclarationSyntax syntax)
            => ClassDeclaration(syntax.Identifier.WithoutTrivia())
                .WithModifiers(TokenList(syntax.Modifiers.Select(x => x.WithoutTrivia())))
                .WithTypeParameterList(syntax.TypeParameterList);

        static StructDeclarationSyntax CopyAsAncestor(StructDeclarationSyntax syntax)
            => StructDeclaration(syntax.Identifier.WithoutTrivia())
                .WithModifiers(TokenList(syntax.Modifiers.Select(x => x.WithoutTrivia())))
                .WithTypeParameterList(syntax.TypeParameterList);
    }
}
