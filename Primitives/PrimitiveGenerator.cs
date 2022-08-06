using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Liversage.Primitives;

[Generator]
public class PrimitiveGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Notice: The definition of Features enum in the following source code
        // should exactly match the definition in this project. The source code
        // isn't generated from reflection to avoid the complexity of getting
        // the XML comments.
        const string source = @"using System;
using System.Diagnostics;

namespace Liversage.Primitives;

[Flags]
public enum Features
{
    /// <summary>
    ///   Implements constructor, ToString() method and conversions to and
    ///   from the inner type.
    /// </summary>
    None,

    /// <summary>
    ///   Implements IEquatable&lt;T&gt; using the == operator for the inner
    ///   type and overrides Equals() and GetHashCode() and implements ==
    ///   and != operators. 
    /// </summary>
    Equatable,

    /// <summary>
    ///   Implements IComparable&lt;T&gt; delegating to the inner type and
    ///   implements &lt;, &lt;=, &gt; and &gt;= operators.
    /// </summary>
    Comparable = Equatable << 1,

    /// <summary>
    ///   Implements IFormattable by delegating to the inner type.
    /// </summary>
    Formattable = Comparable << 1,

    /// <summary>
    ///    Implements TryParse() methods by delegating to the inner type.
    /// </summary>
    Parseable = Formattable << 1,

    /// <summary>
    ///   Implements IConvertible by delegating to the inner type.
    /// </summary>
    Convertible = Parseable << 1,

    Default = Equatable
}

[AttributeUsage(AttributeTargets.Struct)]
[Conditional(""CodeGeneration"")]
public sealed class PrimitiveAttribute : Attribute
{
    public PrimitiveAttribute(Features features = Features.Default) => Features = features;

    public Features Features { get; }

    public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

    public bool MarkAsNonUserCode { get; set; } = true;
}";
        context.RegisterForPostInitialization(postInitializationContext => postInitializationContext.AddSource("PrimitiveAttribute.g.cs", source));
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
            return;

        var progress = new Progress(context);

        const string attributeName = "Liversage.Primitives.PrimitiveAttribute";
        var descriptors = syntaxReceiver.Structs.Select(GetDescriptor).Where(descriptor => descriptor is not null);

        foreach (var descriptor in descriptors)
        {
            var structSyntax = StructSyntax(descriptor!);
            var members =
                !descriptor!.Flags.HasFlag(PrimitiveDescriptorFlags.IsInGlobalNamespace)
                    ? (MemberDeclarationSyntax) FileScopedNamespaceDeclaration(ParseName(descriptor.NamespaceName))
                        .WithMembers(new SyntaxList<MemberDeclarationSyntax>(structSyntax))
                    : structSyntax;
            var compilationUnit = CompilationUnit()
                .AddUsings(GetUsings(descriptor).ToArray())
                .AddMembers(members)
                .NormalizeWhitespace();
            var sourceText = SyntaxTree(compilationUnit, context.ParseOptions, encoding: Encoding.UTF8).GetText();
            context.AddSource($"{descriptor.Name}.g.cs", sourceText);
        }

        PrimitiveDescriptor? GetDescriptor(StructDeclarationSyntax @struct)
        {
            var semanticModel = context.Compilation.GetSemanticModel(@struct.SyntaxTree);
            var descriptor = @struct.ToPrimitiveDescriptor(semanticModel, attributeName, progress);
            if (descriptor is null)
                return null;
            if (descriptor.Features.HasFlag(Features.Comparable) && !descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsComparable))
            {
                progress.Report(Diagnostic.Create(Diagnostics.FieldIsNotComparable, descriptor.InnerLocation));
                descriptor = descriptor with { Features = descriptor.Features & ~Features.Comparable };
            }
            if (descriptor.Features.HasFlag(Features.Parseable) && descriptor.InnerKnownType is KnownType.Unknown or KnownType.String)
            {
                progress.Report(Diagnostic.Create(Diagnostics.FieldIsNotParseable, descriptor.InnerLocation));
                descriptor = descriptor with { Features = descriptor.Features & ~Features.Parseable };
            }
            return descriptor;
        }

        IEnumerable<UsingDirectiveSyntax> GetUsings(PrimitiveDescriptor descriptor)
        {
            yield return UsingDirective(ParseName("System"));
            if (descriptor.MarkAsNonUserCode)
                yield return UsingDirective(ParseName("System.Diagnostics"));
            if (descriptor.Features.HasFlag(Features.Parseable))
                yield return UsingDirective(ParseName("System.Globalization"));
        }
    }

    static StructDeclarationSyntax StructSyntax(PrimitiveDescriptor descriptor)
    {
        var @struct = StructDeclaration(descriptor.Name)
            .WithModifiers(
                TokenList(Token(SyntaxKind.ReadOnlyKeyword), Token(SyntaxKind.PartialKeyword)));

        if (descriptor.MarkAsNonUserCode)
            @struct = @struct
                .WithAttributeLists(
                    SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("DebuggerNonUserCode"))))));

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

    static IEnumerable<BaseTypeSyntax> GetBaseTypes(PrimitiveDescriptor descriptor)
    {
        if (descriptor.Features.HasFlag(Features.Equatable))
            yield return SimpleBaseType(
                GenericName(Identifier("IEquatable"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(descriptor.Name)))));

        if (descriptor.Features.HasFlag(Features.Comparable))
        {
            yield return SimpleBaseType(
                GenericName(Identifier("IComparable"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(descriptor.Name)))));
            yield return SimpleBaseType(IdentifierName("IComparable"));
        }

        if (descriptor.Features.HasFlag(Features.Formattable))
            yield return SimpleBaseType(IdentifierName("IFormattable"));

        if (descriptor.Features.HasFlag(Features.Convertible))
            yield return SimpleBaseType(IdentifierName("IConvertible"));
    }

    static IEnumerable<MemberDeclarationSyntax> GetMembers(PrimitiveDescriptor descriptor)
    {
        if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.HasConstructor))
            yield return descriptor.ConstructorSyntax();

        yield return descriptor.FromPrimitiveSyntax();
        yield return descriptor.ImplicitCastFromPrimitiveSyntax();
        if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString))
        {
            yield return descriptor.ToPrimitiveSyntax();
            yield return descriptor.ExplicitCastToPrimitiveSyntax();
        }
        else
            yield return descriptor.StringExplicitCastToPrimitiveSyntax();

        if (!descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.HasToString))
            yield return !descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString) ? descriptor.ToStringSyntax() : descriptor.StringToStringSyntax();

        if (descriptor.Features.HasFlag(Features.Equatable))
        {
            yield return !descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString)
                ? descriptor.IEquatableTEqualsSyntax()
                : descriptor.StringIEquatableTEqualsSyntax(descriptor.StringComparison);
            yield return descriptor.EqualsSyntax();
            yield return !descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString)
                ? descriptor.GetHashCodeSyntax()
                : descriptor.StringGetHashCodeSyntax(descriptor.StringComparison);
            yield return descriptor.OperatorEqualsSyntax();
            yield return descriptor.OperatorNotEqualsSyntax();
        }

        if (descriptor.Features.HasFlag(Features.Comparable))
        {
            yield return !descriptor.Flags.HasFlag(PrimitiveDescriptorFlags.InnerIsString)
                ? descriptor.IComparableTCompareToSyntax()
                : descriptor.StringIComparableTCompareToSyntax(descriptor.StringComparison);
            yield return descriptor.IComparableCompareToSyntax();
            yield return descriptor.OperatorLessThanSyntax();
            yield return descriptor.OperatorLessThanOrEqualSyntax();
            yield return descriptor.OperatorGreaterThanSyntax();
            yield return descriptor.OperatorGreaterThanOrEqualSyntax();
        }

        if (descriptor.Features.HasFlag(Features.Formattable))
            yield return descriptor.IFormattableToStringSyntax();

        if (descriptor.Features.HasFlag(Features.Parseable))
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

        if (descriptor.Features.HasFlag(Features.Convertible))
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

    class SyntaxReceiver : ISyntaxReceiver
    {
        readonly List<StructDeclarationSyntax> structs = new();

        public IEnumerable<StructDeclarationSyntax> Structs => structs;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is StructDeclarationSyntax { AttributeLists.Count: > 0 } structDeclaration)
                structs.Add(structDeclaration);
        }
    }

    class Progress : IProgress<Diagnostic>
    {
        readonly GeneratorExecutionContext context;

        public Progress(GeneratorExecutionContext context) => this.context = context;

        public void Report(Diagnostic value) => context.ReportDiagnostic(value);
    }
}
