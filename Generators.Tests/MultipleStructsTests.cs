using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Liversage.Primitives.Tests;

public class MultipleStructsTests : TestsBase
{
    [Fact]
    public void Test()
    {
        const string source =
            """
            using Liversage.Primitives;

            namespace Generated;

            [Primitive]
            public readonly partial struct A { readonly int value; }
            [Primitive]
            public readonly partial struct B { readonly int value; }
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
        var generatedSourceA = generatorResult.GeneratedSources[0];
        var generatedSourceB = generatorResult.GeneratedSources[1];
        Assert.Equal("Generated.A.g.cs", generatedSourceA.HintName);
        Assert.Equal("Generated.B.g.cs", generatedSourceB.HintName);
    }
}
