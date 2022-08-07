using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;

namespace Liversage.Primitives.Tests;

public class TestsBase
{
    protected static Compilation CreateCompilation(string source)
    {
        var coreLibPath = typeof(Attribute).Assembly.Location;
        var directoryName = Path.GetDirectoryName(coreLibPath)!;
        var netStandardPath = Path.Combine(directoryName, "netstandard.dll");
        var runtimePath = Path.Combine(directoryName, "System.Runtime.dll");
        return CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[]
            {
                MetadataReference.CreateFromFile(coreLibPath),
                MetadataReference.CreateFromFile(netStandardPath),
                MetadataReference.CreateFromFile(runtimePath),
                MetadataReference.CreateFromFile(typeof(PrimitiveAttribute).Assembly.Location)
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
