using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Reflection;

namespace Dunet.Test;

public class DiscriminatedUnionGeneratorTests
{
    [Fact]
    public void Compiles()
    {
        // Arrange.
        var source = @"
namespace Dunet.Cli;

[Union]
public interface IShape
{
    IShape Circle(double radius);
    IShape Rectangle(double length, double width);
    IShape Triangle(double @base, double height);
}";
        // Act.
        var (generation, compilation) = RunGenerator(source);

        // Assert.
        Assert.Empty(generation.Diagnostics);
        Assert.Empty(compilation.Diagnostics);
    }

    [Fact]
    public void GeneratesUnionTypesInSameNamespace()
    {
        // Arrange.
        const string expectedNamespace = "Test.Namespace";
        var source = @$"
using Dunet;

namespace {expectedNamespace};

[Union]
public interface IShape
{{
    IShape Circle(double radius);
    IShape Rectangle(double length, double width);
    IShape Triangle(double @base, double height);
}}";

        // Act.
        var (generation, compilation) = RunGenerator(source);

        // Assert.
        Assert.Empty(generation.Diagnostics);
        Assert.Empty(compilation.Diagnostics);
    }

    private static Compilation Compile(string source) =>
        CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    private static (GeneratorResult, CompilationResult) RunGenerator(string source)
    {
        var compilation = Compile(source);
        var generator = new DiscriminatedUnionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generationDiagnostics);
        var generationOutput = driver.GetRunResult();
        var compilationDiagnostics = outputCompilation.GetDiagnostics();
        var generatorResult = new GeneratorResult(generationOutput, generationDiagnostics);
        var compilationResult = new CompilationResult(outputCompilation, compilationDiagnostics);
        return (generatorResult, compilationResult);
    }

    record GeneratorResult(GeneratorDriverRunResult Output, ImmutableArray<Diagnostic> Diagnostics);

    record CompilationResult(Compilation Output, ImmutableArray<Diagnostic> Diagnostics);
}