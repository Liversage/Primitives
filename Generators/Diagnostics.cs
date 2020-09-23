using Microsoft.CodeAnalysis;

namespace Liversage.Primitives.Generators
{
    static class Diagnostics
    {
        const string Category = "CodeGeneration";

        public readonly static DiagnosticDescriptor NotAStruct = new DiagnosticDescriptor(
            "LPG001",
            "Not a struct",
            "Type is not a struct.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public readonly static DiagnosticDescriptor NotExactlyOneField = new DiagnosticDescriptor(
            "LPG002",
            "Not exactly one field",
            "Type does not contain exactly one field.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public readonly static DiagnosticDescriptor FieldIsNonStringReference = new DiagnosticDescriptor(
            "LPG003",
            "Field is non-string reference type",
            "The field type cannot be a reference type unless it is string.",
            Category,
            DiagnosticSeverity.Error,
            true);

        public readonly static DiagnosticDescriptor FieldIsNotComparable = new DiagnosticDescriptor(
            "LPG004",
            "Field is not comparable",
            "The field does not implement System.IComparable<T>.",
            Category,
            DiagnosticSeverity.Warning,
            true);

        public readonly static DiagnosticDescriptor FieldIsNotParsable = new DiagnosticDescriptor(
            "LPG005",
            "Field is not parsable",
            "The field is not one of the (possibly nullable) types sbyte, byte, short, ushort, int, uint, long, ulong, single, double, decimal, DateTime, DateTimeOffset or TimeSpan.",
            Category,
            DiagnosticSeverity.Warning,
            true);
    }
}
