using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Liversage.Primitives.Generators
{
    static class PrimitiveDescriptorExtensions
    {
        static TypeSyntax ReadOnlySpanChar
            => GenericName(Identifier("ReadOnlySpan")).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(PredefinedType(Token(SyntaxKind.CharKeyword)))));

        public static bool IsInnerTypeNullableReferenceType(this PrimitiveDescriptor descriptor)
            => descriptor.InnerType is NullableTypeSyntax && descriptor.InnerTypeSymbol.IsReferenceType;

        public static MemberDeclarationSyntax ConstructorSyntax(this PrimitiveDescriptor descriptor)
            => ConstructorDeclaration(descriptor.Name)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(descriptor.InnerName).WithType(descriptor.InnerType))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                ThisExpression(),
                                IdentifierName(descriptor.InnerName)),
                            IdentifierName(descriptor.InnerName))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax FromPrimitiveSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(IdentifierName(descriptor.Name), Identifier("From" + descriptor.InnerTypeName))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                    .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(descriptor.InnerName).WithType(descriptor.InnerType))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        ObjectCreationExpression(IdentifierName(descriptor.Name))
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName(descriptor.InnerName)))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax ImplictCastFromPrimitiveSyntax(this PrimitiveDescriptor descriptor)
            => ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword), IdentifierName(descriptor.Name))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(descriptor.InnerName).WithType(descriptor.InnerType))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(IdentifierName("From" + descriptor.InnerTypeName))
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName(descriptor.InnerName)))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax ToPrimitiveSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(descriptor.InnerType, Identifier("To" + descriptor.InnerTypeName))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithExpressionBody(ArrowExpressionClause(IdentifierName(descriptor.InnerName)))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax ExplicitCastToPrimitiveSyntax(this PrimitiveDescriptor descriptor)
            => ConversionOperatorDeclaration(Token(SyntaxKind.ExplicitKeyword), descriptor.InnerType)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(descriptor.InnerName).WithType(IdentifierName(descriptor.Name)))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(descriptor.InnerName),
                                IdentifierName("To" + descriptor.InnerTypeName)))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax StringExplicitCastToPrimitiveSyntax(this PrimitiveDescriptor descriptor)
            => ConversionOperatorDeclaration(Token(SyntaxKind.ExplicitKeyword), descriptor.InnerType)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("value")).WithType(IdentifierName(descriptor.Name)))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("value"),
                            IdentifierName(descriptor.InnerName))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax ToStringSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Identifier("ToString"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(descriptor.InnerName),
                                IdentifierName("ToString")))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax StringToStringSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Identifier("ToString"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        BinaryExpression(
                            SyntaxKind.CoalesceExpression,
                            IdentifierName(descriptor.InnerName),
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                PredefinedType(Token(SyntaxKind.StringKeyword)),
                                IdentifierName("Empty")))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax IEquatableTEqualsSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("Equals"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("other")).WithType(IdentifierName(descriptor.Name)))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        BinaryExpression(
                            SyntaxKind.EqualsExpression,
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                ThisExpression(),
                                IdentifierName(descriptor.InnerName)),
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("other"),
                                IdentifierName(descriptor.InnerName)))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax StringIEquatableTEqualsSyntax(this PrimitiveDescriptor descriptor, StringComparison stringComparison)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("Equals"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("other")).WithType(IdentifierName(descriptor.Name)))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                PredefinedType(Token(SyntaxKind.StringKeyword)),
                                IdentifierName("Equals")))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression(),
                                                IdentifierName(descriptor.InnerName))),
                                        Token(SyntaxKind.CommaToken),
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("other"),
                                                IdentifierName(descriptor.InnerName))),
                                        Token(SyntaxKind.CommaToken),
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("StringComparison"),
                                                IdentifierName(stringComparison.ToString())))
                                    })))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax EqualsSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("Equals"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(Identifier("obj"))
                                .WithType(descriptor.TryWrapInNullableTypeSyntax(PredefinedType(Token(SyntaxKind.ObjectKeyword)))))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        BinaryExpression(
                            SyntaxKind.LogicalAndExpression,
                            IsPatternExpression(
                                IdentifierName("obj"),
                                DeclarationPattern(
                                    IdentifierName(descriptor.Name),
                                    SingleVariableDesignation(Identifier("value")))),
                            InvocationExpression(IdentifierName("Equals"))
                                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("value"))))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax GetHashCodeSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.IntKeyword)), Identifier("GetHashCode"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(descriptor.InnerName),
                                IdentifierName("GetHashCode")))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax StringGetHashCodeSyntax(this PrimitiveDescriptor descriptor, StringComparison stringComparison)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.IntKeyword)), Identifier("GetHashCode"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        ConditionalExpression(
                            BinaryExpression(
                                SyntaxKind.NotEqualsExpression,
                                IdentifierName(descriptor.InnerName),
                                LiteralExpression(SyntaxKind.NullLiteralExpression)),
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("StringComparer"),
                                        IdentifierName(stringComparison.ToString())),
                                    IdentifierName("GetHashCode")))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(Argument(IdentifierName(descriptor.InnerName))))),
                            LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax OperatorEqualsSyntax(this PrimitiveDescriptor descriptor)
            => OperatorDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Token(SyntaxKind.EqualsEqualsToken))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("value1")).WithType(IdentifierName(descriptor.Name)),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("value2")).WithType(IdentifierName(descriptor.Name))
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("value1"),
                                IdentifierName("Equals")))
                        .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("value2")))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax OperatorNotEqualsSyntax(this PrimitiveDescriptor descriptor)
            => OperatorDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Token(SyntaxKind.ExclamationEqualsToken))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("value1")).WithType(IdentifierName(descriptor.Name)),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("value2")).WithType(IdentifierName(descriptor.Name))
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            ParenthesizedExpression(
                                BinaryExpression(
                                    SyntaxKind.EqualsExpression,
                                    IdentifierName("value1"),
                                    IdentifierName("value2"))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax IComparableTCompareToSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.IntKeyword)), Identifier("CompareTo"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(
                    ParameterList(SingletonSeparatedList(Parameter(Identifier("other")).WithType(IdentifierName(descriptor.Name)))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    ThisExpression(),
                                    IdentifierName(descriptor.InnerName)),
                                IdentifierName("CompareTo")))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("other"),
                                            IdentifierName(descriptor.InnerName))))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax StringIComparableTCompareToSyntax(this PrimitiveDescriptor descriptor, StringComparison stringComparison)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.IntKeyword)), Identifier("CompareTo"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("other")).WithType(IdentifierName(descriptor.Name)))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                PredefinedType(Token(SyntaxKind.StringKeyword)),
                                IdentifierName("Compare")))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression(),
                                                IdentifierName(descriptor.InnerName))),
                                        Token(SyntaxKind.CommaToken),
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("other"),
                                                IdentifierName(descriptor.InnerName))),
                                        Token(SyntaxKind.CommaToken),
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("StringComparison"),
                                                IdentifierName(stringComparison.ToString())))
                                    })))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax IComparableCompareToSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.IntKeyword)), Identifier("CompareTo"))
                .WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifier(IdentifierName("IComparable")))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("obj")).WithType(PredefinedType(Token(SyntaxKind.ObjectKeyword))))))
                .WithBody(
                    Block(
                        IfStatement(
                            PrefixUnaryExpression(
                                SyntaxKind.LogicalNotExpression,
                                ParenthesizedExpression(
                                    IsPatternExpression(
                                        IdentifierName("obj"),
                                        DeclarationPattern(
                                            IdentifierName(descriptor.Name),
                                            SingleVariableDesignation(Identifier("other")))))),
                            ThrowStatement(
                                ObjectCreationExpression(IdentifierName("ArgumentException"))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal($"Object must be of type {descriptor.Name}")))))))),
                        ReturnStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        ThisExpression(),
                                        IdentifierName(descriptor.InnerName)),
                                    IdentifierName("CompareTo")))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("other"),
                                                IdentifierName(descriptor.InnerName)))))))));

        public static MemberDeclarationSyntax OperatorLessThanSyntax(this PrimitiveDescriptor descriptor)
            => OperatorDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Token(SyntaxKind.LessThanToken))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("value1")).WithType(IdentifierName(descriptor.Name)),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("value2")).WithType(IdentifierName(descriptor.Name))
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        BinaryExpression(
                            SyntaxKind.LessThanExpression,
                            InvocationExpression(
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("value1"), IdentifierName("CompareTo")))
                                    .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("value2"))))),
                            LiteralExpression(
                                SyntaxKind.NumericLiteralExpression,
                                Literal(0)))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax OperatorLessThanOrEqualSyntax(this PrimitiveDescriptor descriptor)
            => OperatorDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Token(SyntaxKind.LessThanEqualsToken))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("value1")).WithType(IdentifierName(descriptor.Name)),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("value2")).WithType(IdentifierName(descriptor.Name))
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        BinaryExpression(
                            SyntaxKind.LessThanOrEqualExpression,
                            InvocationExpression(
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("value1"), IdentifierName("CompareTo")))
                                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("value2"))))),
                            LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(0)))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax OperatorGreaterThanSyntax(this PrimitiveDescriptor descriptor)
            => OperatorDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Token(SyntaxKind.GreaterThanToken))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("value1"))
                                    .WithType(IdentifierName(descriptor.Name)),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("value2"))
                                    .WithType(IdentifierName(descriptor.Name))
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            ParenthesizedExpression(
                                BinaryExpression(
                                    SyntaxKind.LessThanOrEqualExpression,
                                    IdentifierName("value1"),
                                    IdentifierName("value2"))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax OperatorGreaterThanOrEqualSyntax(this PrimitiveDescriptor descriptor)
            => OperatorDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Token(SyntaxKind.GreaterThanEqualsToken))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("value1"))
                                    .WithType(IdentifierName(descriptor.Name)),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("value2"))
                                    .WithType(IdentifierName(descriptor.Name))
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            ParenthesizedExpression(
                                BinaryExpression(
                                    SyntaxKind.LessThanExpression,
                                    IdentifierName("value1"),
                                    IdentifierName("value2"))))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax IFormattableToStringSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Identifier("ToString"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("format"))
                                    .WithType(PredefinedType( Token(SyntaxKind.StringKeyword))),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("formatProvider"))
                                    .WithType(IdentifierName("IFormatProvider"))
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    ThisExpression(),
                                    IdentifierName(descriptor.InnerName)),
                                IdentifierName("ToString")))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        Argument(IdentifierName("format")),
                                        Token(SyntaxKind.CommaToken),
                                        Argument(IdentifierName("formatProvider"))
                                    })))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax IConvertibleGetTypeCodeSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(IdentifierName("TypeCode"), Identifier("GetTypeCode"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("Convert"),
                                IdentifierName("GetTypeCode")))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            ThisExpression(),
                                            IdentifierName(descriptor.InnerName))))))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax IConvertibleConvertToSyntax(this PrimitiveDescriptor descriptor, SyntaxKind returnType, string name)
            => descriptor.IConvertibleConvertToSyntax(PredefinedType(Token(returnType)), name);

        public static MemberDeclarationSyntax IConvertibleConvertToSyntax(this PrimitiveDescriptor descriptor, string returnType, string name)
            => descriptor.IConvertibleConvertToSyntax(IdentifierName(returnType), name);

        public static MemberDeclarationSyntax IConvertibleToTypeSyntax(this PrimitiveDescriptor descriptor)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.ObjectKeyword)), Identifier("ToType"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("conversionType"))
                                    .WithType(IdentifierName("Type")),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("provider"))
                                    .WithType(descriptor.TryWrapInNullableTypeSyntax(IdentifierName("IFormatProvider")))
                            })))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("Convert"),
                                IdentifierName("ChangeType")))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression(),
                                                IdentifierName(descriptor.InnerName))),
                                        Token(SyntaxKind.CommaToken),
                                        Argument(IdentifierName("conversionType")),
                                        Token(SyntaxKind.CommaToken),
                                        Argument(IdentifierName("provider"))
                                    })))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        public static MemberDeclarationSyntax NumberTryParseStringSyntax(this PrimitiveDescriptor descriptor)
           => descriptor.NumberTryParseSyntax(PredefinedType(Token(SyntaxKind.StringKeyword)));

        public static MemberDeclarationSyntax NumberTryParseSpanSyntax(this PrimitiveDescriptor descriptor)
           => descriptor.NumberTryParseSyntax(ReadOnlySpanChar);

        public static MemberDeclarationSyntax DateTimeTryParseStringSyntax(this PrimitiveDescriptor descriptor)
           => descriptor.DateTimeTryParseSyntax("DateTimeStyles", PredefinedType(Token(SyntaxKind.StringKeyword)));

        public static MemberDeclarationSyntax DateTimeTryParseSpanSyntax(this PrimitiveDescriptor descriptor)
           => descriptor.DateTimeTryParseSyntax("DateTimeStyles", ReadOnlySpanChar);

        public static MemberDeclarationSyntax TimeSpanTryParseStringSyntax(this PrimitiveDescriptor descriptor)
           => descriptor.DateTimeTryParseSyntax("TimeSpanStyles", PredefinedType(Token(SyntaxKind.StringKeyword)));

        public static MemberDeclarationSyntax TimeSpanTryParseSpanSyntax(this PrimitiveDescriptor descriptor)
           => descriptor.DateTimeTryParseSyntax("TimeSpanStyles", ReadOnlySpanChar);

        public static MemberDeclarationSyntax DateTimeTryParseSyntax(this PrimitiveDescriptor descriptor, string stylesTypeName, TypeSyntax inputType)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("TryParse"))
                .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("s"))
                                    .WithType(inputType),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("format"))
                                    .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("formatProvider"))
                                    .WithType(IdentifierName("IFormatProvider")),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("styles"))
                                    .WithType(IdentifierName(stylesTypeName)),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("value"))
                                    .WithModifiers(TokenList(Token(SyntaxKind.OutKeyword)))
                                    .WithType(IdentifierName(descriptor.Name))
                            })))
                .WithBody(
                    Block(
                        IfStatement(
                            PrefixUnaryExpression(
                                SyntaxKind.LogicalNotExpression,
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(descriptor.InnerTypeName),
                                        IdentifierName("TryParseExact")))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                Argument(IdentifierName("s")),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(IdentifierName("format")),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(IdentifierName("formatProvider")),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(IdentifierName("styles")),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(DeclarationExpression(IdentifierName("var"), SingleVariableDesignation(Identifier("result"))))
                                                    .WithRefKindKeyword(Token(SyntaxKind.OutKeyword))
                                            })))),
                            Block(
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName("value"),
                                        LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)))),
                                ReturnStatement(LiteralExpression(SyntaxKind.FalseLiteralExpression)))),
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName("value"),
                                ObjectCreationExpression(IdentifierName(descriptor.Name))
                                    .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("result"))))))),
                        ReturnStatement(LiteralExpression(SyntaxKind.TrueLiteralExpression))));

        static MemberDeclarationSyntax NumberTryParseSyntax(this PrimitiveDescriptor descriptor, TypeSyntax inputType)
            => MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("TryParse"))
                .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                .WithParameterList(
                    ParameterList(
                        SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                Parameter(Identifier("s"))
                                    .WithType(inputType),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("numberStyles"))
                                    .WithType(IdentifierName("NumberStyles")),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("formatProvider"))
                                    .WithType(IdentifierName("IFormatProvider")),
                                Token(SyntaxKind.CommaToken),
                                Parameter(Identifier("value"))
                                    .WithModifiers(TokenList(Token(SyntaxKind.OutKeyword)))
                                    .WithType(IdentifierName(descriptor.Name))
                            })))
                .WithBody(
                    Block(
                        IfStatement(
                            PrefixUnaryExpression(
                                SyntaxKind.LogicalNotExpression,
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        PredefinedType(Token(descriptor.InnerKnownType.ToTypeKeyword())),
                                        IdentifierName("TryParse")))
                                .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList<ArgumentSyntax>(
                                            new SyntaxNodeOrToken[]
                                            {
                                                Argument(IdentifierName("s")),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(IdentifierName("numberStyles")),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(IdentifierName("formatProvider")),
                                                Token(SyntaxKind.CommaToken),
                                                Argument(
                                                    DeclarationExpression(
                                                        PredefinedType(Token(descriptor.InnerKnownType.ToTypeKeyword())),
                                                        SingleVariableDesignation(Identifier("result"))))
                                                .WithRefKindKeyword(Token(SyntaxKind.OutKeyword))
                                            })))),
                            Block(
                                ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName("value"),
                                        LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)))),
                                ReturnStatement(LiteralExpression(SyntaxKind.FalseLiteralExpression)))),
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName("value"),
                                ObjectCreationExpression(IdentifierName(descriptor.Name))
                                    .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("result"))))))),
                        ReturnStatement(LiteralExpression(SyntaxKind.TrueLiteralExpression))));

        static MemberDeclarationSyntax IConvertibleConvertToSyntax(this PrimitiveDescriptor descriptor, TypeSyntax returnType, string name)
            => MethodDeclaration(returnType, Identifier(name))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(
                    ParameterList(
                        SingletonSeparatedList(
                            Parameter(Identifier("provider"))
                                .WithType(descriptor.TryWrapInNullableTypeSyntax(IdentifierName("IFormatProvider"))))))
                .WithExpressionBody(
                    ArrowExpressionClause(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("Convert"),
                                IdentifierName(name)))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        Argument(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression(),
                                                IdentifierName(descriptor.InnerName))),
                                        Token(SyntaxKind.CommaToken),
                                        Argument(IdentifierName("provider"))
                                    })))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        static TypeSyntax TryWrapInNullableTypeSyntax(this PrimitiveDescriptor descriptor, TypeSyntax typeSyntax)
            => descriptor.IsInnerTypeNullableReferenceType() ? NullableType(typeSyntax) : typeSyntax;
    }
}
