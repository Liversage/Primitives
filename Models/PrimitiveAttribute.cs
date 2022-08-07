using System;

namespace Liversage.Primitives;

[AttributeUsage(AttributeTargets.Struct)]
public sealed class PrimitiveAttribute : Attribute
{
    public PrimitiveAttribute(Features features = Features.Default) => Features = features;

    public Features Features { get; }

    public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

    public bool MarkAsNonUserCode { get; set; } = true;
}
