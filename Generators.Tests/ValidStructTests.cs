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
            "BasedOnInt.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct BasedOnInt : IEquatable<BasedOnInt>
            {
                public BasedOnInt(int id) => this.id = id;
                public static BasedOnInt FromInt32(int id) => new BasedOnInt(id);
                public static implicit operator BasedOnInt(int id) => FromInt32(id);
                public int ToInt32() => id;
                public static explicit operator int (BasedOnInt id) => id.ToInt32();
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
            public readonly partial struct BasedOnInt
            {
                readonly Int32 id;
            }
            """,
            "BasedOnInt.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct BasedOnInt : IEquatable<BasedOnInt>
            {
                public BasedOnInt(Int32 id) => this.id = id;
                public static BasedOnInt FromInt32(Int32 id) => new BasedOnInt(id);
                public static implicit operator BasedOnInt(Int32 id) => FromInt32(id);
                public Int32 ToInt32() => id;
                public static explicit operator Int32(BasedOnInt id) => id.ToInt32();
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

            namespace Generated;

            [Primitive(MarkAsNonUserCode = false)]
            public readonly partial struct BasedOnInt
            {
                readonly int id;
            }
            """,
            "BasedOnInt.g.cs",
            """
            using System;

            namespace Generated;
            readonly partial struct BasedOnInt : IEquatable<BasedOnInt>
            {
                public BasedOnInt(int id) => this.id = id;
                public static BasedOnInt FromInt32(int id) => new BasedOnInt(id);
                public static implicit operator BasedOnInt(int id) => FromInt32(id);
                public int ToInt32() => id;
                public static explicit operator int (BasedOnInt id) => id.ToInt32();
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

            namespace Generated;

            [Primitive]
            public readonly partial struct BasedOnNullableString
            {
                readonly string? value;
            }
            """,
            "BasedOnNullableString.g.cs",
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
            "BasedOnTimeSpan.g.cs",
            """
            using System;
            using System.Diagnostics;
            using System.Globalization;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct BasedOnTimeSpan : IEquatable<BasedOnTimeSpan>, IFormattable
            {
                public BasedOnTimeSpan(TimeSpan duration) => this.duration = duration;
                public static BasedOnTimeSpan FromTimeSpan(TimeSpan duration) => new BasedOnTimeSpan(duration);
                public static implicit operator BasedOnTimeSpan(TimeSpan duration) => FromTimeSpan(duration);
                public TimeSpan ToTimeSpan() => duration;
                public static explicit operator TimeSpan(BasedOnTimeSpan duration) => duration.ToTimeSpan();
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
                public InGlobalNamespace(int id) => this.id = id;
                public static InGlobalNamespace FromInt32(int id) => new InGlobalNamespace(id);
                public static implicit operator InGlobalNamespace(int id) => FromInt32(id);
                public int ToInt32() => id;
                public static explicit operator int (InGlobalNamespace id) => id.ToInt32();
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
            "Comparable.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct Comparable : IEquatable<Comparable>, IComparable<Comparable>, IComparable
            {
                public Comparable(int id) => this.id = id;
                public static Comparable FromInt32(int id) => new Comparable(id);
                public static implicit operator Comparable(int id) => FromInt32(id);
                public int ToInt32() => id;
                public static explicit operator int (Comparable id) => id.ToInt32();
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
            public readonly partial struct BasedOnNullableString
            {
                readonly string? value;
            }
            """,
            "BasedOnNullableString.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            #nullable enable
            [DebuggerNonUserCode]
            readonly partial struct BasedOnNullableString : IEquatable<BasedOnNullableString>, IComparable<BasedOnNullableString>, IComparable
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
                public int CompareTo(BasedOnNullableString other) => string.Compare(this.value, other.value, StringComparison.Ordinal);
                int IComparable.CompareTo(object? obj)
                {
                    if (!(obj is BasedOnNullableString other))
                        throw new ArgumentException("Object must be of type BasedOnNullableString.");
                    if (ReferenceEquals(this.value, other.value))
                        return 0;
                    return this.value?.CompareTo(other.value) ?? -1;
                }

                public static bool operator <(BasedOnNullableString value1, BasedOnNullableString value2) => value1.CompareTo(value2) < 0;
                public static bool operator <=(BasedOnNullableString value1, BasedOnNullableString value2) => value1.CompareTo(value2) <= 0;
                public static bool operator>(BasedOnNullableString value1, BasedOnNullableString value2) => !(value1 <= value2);
                public static bool operator >=(BasedOnNullableString value1, BasedOnNullableString value2) => !(value1 < value2);
            }
            """
        },
        new object[]
        {
            """
            using Liversage.Primitives;

            namespace Generated;

            [Primitive(Features.Default | Features.Comparable)]
            public readonly partial struct BasedOnString
            {
                readonly string value;
            }
            """,
            "BasedOnString.g.cs",
            """
            using System;
            using System.Diagnostics;

            namespace Generated;
            [DebuggerNonUserCode]
            readonly partial struct BasedOnString : IEquatable<BasedOnString>, IComparable<BasedOnString>, IComparable
            {
                public BasedOnString(string value) => this.value = value;
                public static BasedOnString FromString(string value) => new BasedOnString(value);
                public static implicit operator BasedOnString(string value) => FromString(value);
                public static explicit operator string (BasedOnString value) => value.value;
                public override string ToString() => value ?? string.Empty;
                public bool Equals(BasedOnString other) => string.Equals(this.value, other.value, StringComparison.Ordinal);
                public override bool Equals(object obj) => obj is BasedOnString value && Equals(value);
                public override int GetHashCode() => value != null ? StringComparer.Ordinal.GetHashCode(value) : 0;
                public static bool operator ==(BasedOnString value1, BasedOnString value2) => value1.Equals(value2);
                public static bool operator !=(BasedOnString value1, BasedOnString value2) => !(value1 == value2);
                public int CompareTo(BasedOnString other) => string.Compare(this.value, other.value, StringComparison.Ordinal);
                int IComparable.CompareTo(object obj)
                {
                    if (!(obj is BasedOnString other))
                        throw new ArgumentException("Object must be of type BasedOnString.");
                    if (ReferenceEquals(this.value, other.value))
                        return 0;
                    return this.value?.CompareTo(other.value) ?? -1;
                }

                public static bool operator <(BasedOnString value1, BasedOnString value2) => value1.CompareTo(value2) < 0;
                public static bool operator <=(BasedOnString value1, BasedOnString value2) => value1.CompareTo(value2) <= 0;
                public static bool operator>(BasedOnString value1, BasedOnString value2) => !(value1 <= value2);
                public static bool operator >=(BasedOnString value1, BasedOnString value2) => !(value1 < value2);
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
