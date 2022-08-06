using System;

namespace Samples;

public readonly struct FixedPoint : IEquatable<FixedPoint>
{
    public const int Precision = 2;

    const int multiplier = 100; // 10^Precision

    readonly int value;

    public FixedPoint(double value) => this.value = (int) Math.Round(value * multiplier);

    public bool Equals(FixedPoint other) => value == other.value;

    public override bool Equals(object? obj) => obj is FixedPoint fixedPoint && Equals(fixedPoint);

    public override int GetHashCode() => value.GetHashCode();

#pragma warning disable CA1305 // Specify IFormatProvider
    public override string ToString() => ((double) value / multiplier).ToString("F2");
#pragma warning restore CA1305 // Specify IFormatProvider

    public static bool operator ==(FixedPoint left, FixedPoint right) => left.Equals(right);

    public static bool operator !=(FixedPoint left, FixedPoint right) => !(left == right);
}
