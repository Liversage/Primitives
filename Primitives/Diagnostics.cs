using Microsoft.CodeAnalysis;

namespace Liversage.Primitives;

static class Diagnostics
{
    const string category = "CodeGeneration";

    public static readonly DiagnosticDescriptor NotExactlyOneField = new(
        "LPG002",
        "Not exactly one field",
        "Type does not contain exactly one field",
        category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor FieldIsNonStringReference = new(
        "LPG003",
        "Field is non-string reference type",
        "The field type cannot be a reference type unless it is string",
        category,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor FieldIsNotComparable = new(
        "LPG004",
        "Field is not comparable",
        "The field does not implement System.IComparable<T>",
        category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor FieldIsNotParseable = new(
        "LPG005",
        "Field is not parseable",
        "The field is not one of the (possibly nullable) types sbyte, byte, short, ushort, int, uint, long, ulong, single, double, decimal, DateTime, DateTimeOffset or TimeSpan",
        category,
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor InvalidAttributeConstructor = new(
        "LPG006",
        "Attribute is invalid",
        "The PrimitiveAttribute is invalid",
        category,
        DiagnosticSeverity.Error,
        true);
}
