using CodeGeneration.Roslyn;
using System;
using System.Diagnostics;

namespace Liversage.Primitives
{
    [AttributeUsage(AttributeTargets.Struct)]
    [CodeGenerationAttribute("Liversage.Primitives.Generators.PrimitiveGenerator, Liversage.Primitives.Generators")]
    [Conditional("CodeGeneration")]
    public sealed class PrimitiveAttribute : Attribute
    {
        public PrimitiveAttribute(Features features = Features.Default) => Features = features;

        public Features Features { get; }

        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

        public bool MarkAsNonUserCode { get; set; } = true;
    }
}
