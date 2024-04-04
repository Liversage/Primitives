using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Liversage.Primitives.Tests;

public class MultipleNamespacesTests : TestsBase
{
    [Fact]
    public void Test()
    {
        const string source =
            """
            using Liversage.Primitives;

            namespace Generated1
            {
                [Primitive]
                public readonly partial struct Primitive
                {
                    readonly int id;
                }
            }

            namespace Generated2
            {
                [Primitive]
                public readonly partial struct Primitive
                {
                    readonly int id;
                }
            }
            """;
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
        Assert.Equal(2, generatorResult.GeneratedSources.Length);
        var generatedSource1 = generatorResult.GeneratedSources[0];
        var generatedSource2 = generatorResult.GeneratedSources[1];
        Assert.Equal("Generated1.Primitive.g.cs", generatedSource1.HintName);
        Assert.Equal("Generated2.Primitive.g.cs", generatedSource2.HintName);
        const string expectedSourceFormat = """
            using System;
            using System.Diagnostics;

            namespace Generated{0};
            [DebuggerNonUserCode]
            readonly partial struct Primitive : IEquatable<Primitive>
            {{
                public Primitive(int value) => this.id = value;
                public static Primitive FromInt32(int value) => new Primitive(value);
                public static implicit operator Primitive(int value) => FromInt32(value);
                public int ToInt32() => id;
                public static explicit operator int (Primitive value) => value.ToInt32();
                public override string ToString() => id.ToString();
                public bool Equals(Primitive other) => this.id == other.id;
                public override bool Equals(object obj) => obj is Primitive value && Equals(value);
                public override int GetHashCode() => id.GetHashCode();
                public static bool operator ==(Primitive value1, Primitive value2) => value1.Equals(value2);
                public static bool operator !=(Primitive value1, Primitive value2) => !(value1 == value2);
            }}
            """;
        Assert.Equal(string.Format(CultureInfo.InvariantCulture, expectedSourceFormat, 1), generatedSource1.SourceText.ToString());
        Assert.Equal(string.Format(CultureInfo.InvariantCulture, expectedSourceFormat, 2), generatedSource2.SourceText.ToString());
    }
}
