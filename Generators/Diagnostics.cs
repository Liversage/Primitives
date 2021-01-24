using Microsoft.CodeAnalysis;

namespace Liversage.Primitives.Generators
{
    static class Diagnostics
    {
        const string category = "CodeGeneration";

        public readonly static DiagnosticDescriptor NotAStruct = new DiagnosticDescriptor(
            "LPG001",
            "Not a struct",
            "Type is not a struct.",
            category,
            DiagnosticSeverity.Error,
            true);

        public readonly static DiagnosticDescriptor NotExactlyOneField = new DiagnosticDescriptor(
            "LPG002",
            "Not exactly one field",
            "Type does not contain exactly one field.",
            category,
            DiagnosticSeverity.Error,
            true);

        public readonly static DiagnosticDescriptor FieldIsNonStringReference = new DiagnosticDescriptor(
            "LPG003",
            "Field is non-string reference type",
            "The field type cannot be a reference type unless it is string.",
            category,
            DiagnosticSeverity.Error,
            true);

        public readonly static DiagnosticDescriptor FieldIsNotComparable = new DiagnosticDescriptor(
            "LPG004",
            "Field is not comparable",
            "The field does not implement System.IComparable<T>.",
            category,
            DiagnosticSeverity.Warning,
            true);

        public readonly static DiagnosticDescriptor FieldIsNotParseable = new DiagnosticDescriptor(
            "LPG005",
            "Field is not parseable",
            "The field is not one of the (possibly nullable) types sbyte, byte, short, ushort, int, uint, long, ulong, single, double, decimal, DateTime, DateTimeOffset or TimeSpan.",
            category,
            DiagnosticSeverity.Warning,
            true);
    }
}
