using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using Xunit;

namespace Liversage.Primitives.Tests;

public class ValidStructTests : TestsBase
{
    public static IEnumerable<object[]> TestData { get; } = new[]
    {
        new object[]
        {
            """
            using Liversage.Primitives;

            namespace Generated;

            [Primitive]
            public readonly partial struct BasedOnInt
            {
                readonly int id;
            }
            """,
            "Generated.BasedOnInt.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct BasedOnInt : IEquatable<BasedOnInt>
            {
                public BasedOnInt(int value) => this.id = value;
                public static BasedOnInt FromInt32(int value) => new BasedOnInt(value);
                public static implicit operator BasedOnInt(int value) => FromInt32(value);
                public int ToInt32() => id;
                public static explicit operator int (BasedOnInt value) => value.ToInt32();
                public override string ToString() => id.ToString();
                public bool Equals(BasedOnInt other) => this.id == other.id;
                public override bool Equals(object obj) => obj is BasedOnInt value && Equals(value);
                public override int GetHashCode() => id.GetHashCode();
                public static bool operator ==(BasedOnInt value1, BasedOnInt value2) => value1.Equals(value2);
                public static bool operator !=(BasedOnInt value1, BasedOnInt value2) => !(value1 == value2);
            }
            """
        },
        new object[]
        {
            """
            using Liversage.Primitives;
            using System;

            namespace Generated;

            [Primitive]
            public readonly partial struct BasedOnInt32
            {
                readonly Int32 id;
            }
            """,
            "Generated.BasedOnInt32.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct BasedOnInt32 : IEquatable<BasedOnInt32>
            {
                public BasedOnInt32(Int32 value) => this.id = value;
                public static BasedOnInt32 FromInt32(Int32 value) => new BasedOnInt32(value);
                public static implicit operator BasedOnInt32(Int32 value) => FromInt32(value);
                public Int32 ToInt32() => id;
                public static explicit operator Int32(BasedOnInt32 value) => value.ToInt32();
                public override string ToString() => id.ToString();
                public bool Equals(BasedOnInt32 other) => this.id == other.id;
                public override bool Equals(object obj) => obj is BasedOnInt32 value && Equals(value);
                public override int GetHashCode() => id.GetHashCode();
                public static bool operator ==(BasedOnInt32 value1, BasedOnInt32 value2) => value1.Equals(value2);
                public static bool operator !=(BasedOnInt32 value1, BasedOnInt32 value2) => !(value1 == value2);
            }
            """
        },
        new object[]
        {
            """
            using Liversage.Primitives;

            namespace Generated;

            [Primitive(MarkAsNonUserCode = false)]
            public readonly partial struct DontMarkAsNonUserCode
            {
                readonly int id;
            }
            """,
            "Generated.DontMarkAsNonUserCode.g.cs",
            """
            using System;

            namespace Generated;
            readonly partial struct DontMarkAsNonUserCode : IEquatable<DontMarkAsNonUserCode>
            {
                public DontMarkAsNonUserCode(int value) => this.id = value;
                public static DontMarkAsNonUserCode FromInt32(int value) => new DontMarkAsNonUserCode(value);
                public static implicit operator DontMarkAsNonUserCode(int value) => FromInt32(value);
                public int ToInt32() => id;
                public static explicit operator int (DontMarkAsNonUserCode value) => value.ToInt32();
                public override string ToString() => id.ToString();
                public bool Equals(DontMarkAsNonUserCode other) => this.id == other.id;
                public override bool Equals(object obj) => obj is DontMarkAsNonUserCode value && Equals(value);
                public override int GetHashCode() => id.GetHashCode();
                public static bool operator ==(DontMarkAsNonUserCode value1, DontMarkAsNonUserCode value2) => value1.Equals(value2);
                public static bool operator !=(DontMarkAsNonUserCode value1, DontMarkAsNonUserCode value2) => !(value1 == value2);
            }
            """
        },
        new object[]
        {
            """
            using Liversage.Primitives;

            namespace Generated;

            [Primitive]
            public readonly partial struct BasedOnNullableString
            {
                readonly string? value;
            }
            """,
            "Generated.BasedOnNullableString.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            #nullable enable
            [DebuggerNonUserCode]
            readonly partial struct BasedOnNullableString : IEquatable<BasedOnNullableString>
            {
                public BasedOnNullableString(string? value) => this.value = value;
                public static BasedOnNullableString FromString(string? value) => new BasedOnNullableString(value);
                public static implicit operator BasedOnNullableString(string? value) => FromString(value);
                public static explicit operator string? (BasedOnNullableString value) => value.value;
                public override string ToString() => value ?? string.Empty;
                public bool Equals(BasedOnNullableString other) => string.Equals(this.value, other.value, StringComparison.Ordinal);
                public override bool Equals(object? obj) => obj is BasedOnNullableString value && Equals(value);
                public override int GetHashCode() => value != null ? StringComparer.Ordinal.GetHashCode(value) : 0;
                public static bool operator ==(BasedOnNullableString value1, BasedOnNullableString value2) => value1.Equals(value2);
                public static bool operator !=(BasedOnNullableString value1, BasedOnNullableString value2) => !(value1 == value2);
            }
            """
        },
        new object[]
        {
            """
            using Liversage.Primitives;
            using System;

            namespace Generated
            {
                [Primitive(Features.Default | Features.Formattable | Features.Parseable)]
                public readonly partial struct BasedOnTimeSpan
                {
                    readonly TimeSpan duration;
                }
            }
            """,
            "Generated.BasedOnTimeSpan.g.cs",
            """
            using System;
            using System.Diagnostics;
            using System.Globalization;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct BasedOnTimeSpan : IEquatable<BasedOnTimeSpan>, IFormattable
            {
                public BasedOnTimeSpan(TimeSpan value) => this.duration = value;
                public static BasedOnTimeSpan FromTimeSpan(TimeSpan value) => new BasedOnTimeSpan(value);
                public static implicit operator BasedOnTimeSpan(TimeSpan value) => FromTimeSpan(value);
                public TimeSpan ToTimeSpan() => duration;
                public static explicit operator TimeSpan(BasedOnTimeSpan value) => value.ToTimeSpan();
                public override string ToString() => duration.ToString();
                public bool Equals(BasedOnTimeSpan other) => this.duration == other.duration;
                public override bool Equals(object obj) => obj is BasedOnTimeSpan value && Equals(value);
                public override int GetHashCode() => duration.GetHashCode();
                public static bool operator ==(BasedOnTimeSpan value1, BasedOnTimeSpan value2) => value1.Equals(value2);
                public static bool operator !=(BasedOnTimeSpan value1, BasedOnTimeSpan value2) => !(value1 == value2);
                public string ToString(string format, IFormatProvider formatProvider) => this.duration.ToString(format, formatProvider);
                public static bool TryParse(string s, string format, IFormatProvider formatProvider, TimeSpanStyles styles, out BasedOnTimeSpan value)
                {
                    if (!TimeSpan.TryParseExact(s, format, formatProvider, styles, out var result))
                    {
                        value = default;
                        return false;
                    }

                    value = new BasedOnTimeSpan(result);
                    return true;
                }

                public static bool TryParse(ReadOnlySpan<char> s, string format, IFormatProvider formatProvider, TimeSpanStyles styles, out BasedOnTimeSpan value)
                {
                    if (!TimeSpan.TryParseExact(s, format, formatProvider, styles, out var result))
                    {
                        value = default;
                        return false;
                    }

                    value = new BasedOnTimeSpan(result);
                    return true;
                }
            }
            """
        },
        new object[]
        {
            """
            using Liversage.Primitives;

            [Primitive]
            public readonly partial struct InGlobalNamespace
            {
                readonly int id;
            }
            """,
            "InGlobalNamespace.g.cs",
            """
            using System;
            using System.Diagnostics;

            [DebuggerNonUserCode]
            readonly partial struct InGlobalNamespace : IEquatable<InGlobalNamespace>
            {
                public InGlobalNamespace(int value) => this.id = value;
                public static InGlobalNamespace FromInt32(int value) => new InGlobalNamespace(value);
                public static implicit operator InGlobalNamespace(int value) => FromInt32(value);
                public int ToInt32() => id;
                public static explicit operator int (InGlobalNamespace value) => value.ToInt32();
                public override string ToString() => id.ToString();
                public bool Equals(InGlobalNamespace other) => this.id == other.id;
                public override bool Equals(object obj) => obj is InGlobalNamespace value && Equals(value);
                public override int GetHashCode() => id.GetHashCode();
                public static bool operator ==(InGlobalNamespace value1, InGlobalNamespace value2) => value1.Equals(value2);
                public static bool operator !=(InGlobalNamespace value1, InGlobalNamespace value2) => !(value1 == value2);
            }
            """
        },
        new object[]
        {
            """
            using Liversage.Primitives;

            namespace Generated;

            [Primitive(Features.Default | Features.Comparable)]
            public readonly partial struct Comparable
            {
                readonly int id;
            }
            """,
            "Generated.Comparable.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct Comparable : IEquatable<Comparable>, IComparable<Comparable>, IComparable
            {
                public Comparable(int value) => this.id = value;
                public static Comparable FromInt32(int value) => new Comparable(value);
                public static implicit operator Comparable(int value) => FromInt32(value);
                public int ToInt32() => id;
                public static explicit operator int (Comparable value) => value.ToInt32();
                public override string ToString() => id.ToString();
                public bool Equals(Comparable other) => this.id == other.id;
                public override bool Equals(object obj) => obj is Comparable value && Equals(value);
                public override int GetHashCode() => id.GetHashCode();
                public static bool operator ==(Comparable value1, Comparable value2) => value1.Equals(value2);
                public static bool operator !=(Comparable value1, Comparable value2) => !(value1 == value2);
                public int CompareTo(Comparable other) => this.id.CompareTo(other.id);
                int IComparable.CompareTo(object obj)
                {
                    if (!(obj is Comparable other))
                        throw new ArgumentException("Object must be of type Comparable.");
                    return this.id.CompareTo(other.id);
                }

                public static bool operator <(Comparable value1, Comparable value2) => value1.CompareTo(value2) < 0;
                public static bool operator <=(Comparable value1, Comparable value2) => value1.CompareTo(value2) <= 0;
                public static bool operator>(Comparable value1, Comparable value2) => !(value1 <= value2);
                public static bool operator >=(Comparable value1, Comparable value2) => !(value1 < value2);
            }
            """
        },
        new object[]
        {
            """
            using Liversage.Primitives;

            namespace Generated;

            [Primitive(Features.Default | Features.Comparable)]
            public readonly partial struct ComparableBasedOnNullableString
            {
                readonly string? value;
            }
            """,
            "Generated.ComparableBasedOnNullableString.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            #nullable enable
            [DebuggerNonUserCode]
            readonly partial struct ComparableBasedOnNullableString : IEquatable<ComparableBasedOnNullableString>, IComparable<ComparableBasedOnNullableString>, IComparable
            {
                public ComparableBasedOnNullableString(string? value) => this.value = value;
                public static ComparableBasedOnNullableString FromString(string? value) => new ComparableBasedOnNullableString(value);
                public static implicit operator ComparableBasedOnNullableString(string? value) => FromString(value);
                public static explicit operator string? (ComparableBasedOnNullableString value) => value.value;
                public override string ToString() => value ?? string.Empty;
                public bool Equals(ComparableBasedOnNullableString other) => string.Equals(this.value, other.value, StringComparison.Ordinal);
                public override bool Equals(object? obj) => obj is ComparableBasedOnNullableString value && Equals(value);
                public override int GetHashCode() => value != null ? StringComparer.Ordinal.GetHashCode(value) : 0;
                public static bool operator ==(ComparableBasedOnNullableString value1, ComparableBasedOnNullableString value2) => value1.Equals(value2);
                public static bool operator !=(ComparableBasedOnNullableString value1, ComparableBasedOnNullableString value2) => !(value1 == value2);
                public int CompareTo(ComparableBasedOnNullableString other) => string.Compare(this.value, other.value, StringComparison.Ordinal);
                int IComparable.CompareTo(object? obj)
                {
                    if (!(obj is ComparableBasedOnNullableString other))
                        throw new ArgumentException("Object must be of type ComparableBasedOnNullableString.");
                    if (ReferenceEquals(this.value, other.value))
                        return 0;
                    return this.value?.CompareTo(other.value) ?? -1;
                }

                public static bool operator <(ComparableBasedOnNullableString value1, ComparableBasedOnNullableString value2) => value1.CompareTo(value2) < 0;
                public static bool operator <=(ComparableBasedOnNullableString value1, ComparableBasedOnNullableString value2) => value1.CompareTo(value2) <= 0;
                public static bool operator>(ComparableBasedOnNullableString value1, ComparableBasedOnNullableString value2) => !(value1 <= value2);
                public static bool operator >=(ComparableBasedOnNullableString value1, ComparableBasedOnNullableString value2) => !(value1 < value2);
            }
            """
        },
        new object[]
        {
            """
            using Liversage.Primitives;

            namespace Generated;

            [Primitive(Features.Default | Features.Comparable)]
            public readonly partial struct ComparableBasedOnString
            {
                readonly string value;
            }
            """,
            "Generated.ComparableBasedOnString.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct ComparableBasedOnString : IEquatable<ComparableBasedOnString>, IComparable<ComparableBasedOnString>, IComparable
            {
                public ComparableBasedOnString(string value) => this.value = value;
                public static ComparableBasedOnString FromString(string value) => new ComparableBasedOnString(value);
                public static implicit operator ComparableBasedOnString(string value) => FromString(value);
                public static explicit operator string (ComparableBasedOnString value) => value.value;
                public override string ToString() => value ?? string.Empty;
                public bool Equals(ComparableBasedOnString other) => string.Equals(this.value, other.value, StringComparison.Ordinal);
                public override bool Equals(object obj) => obj is ComparableBasedOnString value && Equals(value);
                public override int GetHashCode() => value != null ? StringComparer.Ordinal.GetHashCode(value) : 0;
                public static bool operator ==(ComparableBasedOnString value1, ComparableBasedOnString value2) => value1.Equals(value2);
                public static bool operator !=(ComparableBasedOnString value1, ComparableBasedOnString value2) => !(value1 == value2);
                public int CompareTo(ComparableBasedOnString other) => string.Compare(this.value, other.value, StringComparison.Ordinal);
                int IComparable.CompareTo(object obj)
                {
                    if (!(obj is ComparableBasedOnString other))
                        throw new ArgumentException("Object must be of type ComparableBasedOnString.");
                    if (ReferenceEquals(this.value, other.value))
                        return 0;
                    return this.value?.CompareTo(other.value) ?? -1;
                }

                public static bool operator <(ComparableBasedOnString value1, ComparableBasedOnString value2) => value1.CompareTo(value2) < 0;
                public static bool operator <=(ComparableBasedOnString value1, ComparableBasedOnString value2) => value1.CompareTo(value2) <= 0;
                public static bool operator>(ComparableBasedOnString value1, ComparableBasedOnString value2) => !(value1 <= value2);
                public static bool operator >=(ComparableBasedOnString value1, ComparableBasedOnString value2) => !(value1 < value2);
            }
            """
        }
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void Test(string source, string expectedGeneratedFileName, string expectedGeneratedSource)
    {
        var inputCompilation = CreateCompilation(source);

        var generator = new PrimitiveGenerator();
        var driver = CSharpGeneratorDriver.Create(generator).RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _);
        var runResult = driver.GetRunResult();

        Assert.True(runResult.Diagnostics.IsEmpty);
        Assert.Single(runResult.Results);
        var generatorResult = runResult.Results[0];
        Assert.Equal(generator, generatorResult.Generator);
        Assert.True(generatorResult.Diagnostics.IsEmpty);
        Assert.Null(generatorResult.Exception);
        Assert.Single(generatorResult.GeneratedSources);
        var generatedSource = generatorResult.GeneratedSources[0];
        Assert.Equal(expectedGeneratedFileName, generatedSource.HintName);
        var generatedSourceText = generatedSource.SourceText.ToString();
        Assert.Equal(expectedGeneratedSource, generatedSourceText);
    }
}
