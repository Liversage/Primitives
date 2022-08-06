using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using Xunit;

namespace Liversage.Primitives.Tests;

public class InvalidStructTests : TestsBase
{
    public static IEnumerable<object[]> TestData { get; } = new[]
    {
        new object[]
        {
            @"using Liversage.Primitives;

namespace Generated;

[Primitive]
public readonly partial struct TwoFields
{
    readonly int id;
    readonly int id2;
}",
            "LPG002"
        },
        new object[]
        {
            @"using Liversage.Primitives;

namespace Generated;

[Primitive]
public readonly partial struct TwoFields
{
    readonly int id, id2;
}",
            "LPG002"
        },
        new object[]
        {
            @"using Liversage.Primitives;

namespace Generated;

[Primitive]
public readonly partial struct ReferenceTypeNotString
{
    readonly object? value;
}",
            "LPG003"
        },
        new object[]
        {
            @"using Liversage.Primitives;

namespace Generated;

[Primitive(Features.Default, Features.Comparable)]
public readonly partial struct InvalidAttribute
{
    readonly object? value;
}",
            "LPG006"
        },
        new object[]
        {
            @"using Liversage.Primitives;

namespace Generated;

[Primitive(""Default"")]
public readonly partial struct InvalidAttribute
{
    readonly object? value;
}",
            "LPG006"
        },
        new object[]
        {
            @"using Liversage.Primitives;

namespace Generated;

[Primitive(StringComparison = ""Ordinal"")]
public readonly partial struct InvalidAttribute
{
    readonly object? value;
}",
            "LPG006"
        },
        new object[]
        {
            @"using Liversage.Primitives;

namespace Generated;

[Primitive(MarkAsNonUserCode = 123)]
public readonly partial struct InvalidAttribute
{
    readonly object? value;
}",
            "LPG006"
        }
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void Test(string source, string expectedDiagnosticId)
    {
        var inputCompilation = CreateCompilation(source);

        var generator = new PrimitiveGenerator();
        var driver = CSharpGeneratorDriver.Create(generator).RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _);
        var runResult = driver.GetRunResult();

        Assert.Single(runResult.Diagnostics);
        var diagnostic = runResult.Diagnostics[0];
        Assert.Equal(expectedDiagnosticId, diagnostic.Id);
    }
}
