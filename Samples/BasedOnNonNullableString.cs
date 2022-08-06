using Liversage.Primitives;
using System;

namespace Samples;

[Primitive]
public readonly partial struct BasedOnNonNullableString
{
    readonly string value;

    public BasedOnNonNullableString(string? value) => this.value = value ?? throw new ArgumentNullException(nameof(value));
}
