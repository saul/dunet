﻿using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dunet.Test.Compiler;

class DictionaryAnalyzerConfigOptions : AnalyzerConfigOptions
{
    public static DictionaryAnalyzerConfigOptions Empty { get; } = new(ImmutableDictionary.Create<string, string>(KeyComparer));

    public ImmutableDictionary<string, string> Options { get; }

    public DictionaryAnalyzerConfigOptions(ImmutableDictionary<string, string> options)
        => Options = options;

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        => Options.TryGetValue(key, out value);
}

class DictionaryAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    public static DictionaryAnalyzerConfigOptionsProvider Empty { get; } = new(DictionaryAnalyzerConfigOptions.Empty);

    internal DictionaryAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions)
    {
        GlobalOptions = globalOptions;
    }

    public override AnalyzerConfigOptions GlobalOptions { get; }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        => DictionaryAnalyzerConfigOptions.Empty;

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        => DictionaryAnalyzerConfigOptions.Empty;
}