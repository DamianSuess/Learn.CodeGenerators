using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeGen.Extensions;

/// <summary><inheritdoc cref="GeneratorAttributeSyntaxContext" path="/summary/node()"/></summary>
/// <param name="syntaxContext">The original <see cref="GeneratorAttributeSyntaxContext"/> value.</param>
/// <param name="globalOptions">The original <see cref="AnalyzerConfigOptions"/> value.</param>
internal readonly struct GeneratorAttributeSyntaxContextWithOptions(
  GeneratorAttributeSyntaxContext syntaxContext,
  AnalyzerConfigOptions globalOptions)
{
  /// <inheritdoc cref="GeneratorAttributeSyntaxContext.Attributes"/>
  public ImmutableArray<AttributeData> Attributes { get; } = syntaxContext.Attributes;

  /// <inheritdoc cref="AnalyzerConfigOptionsProvider.GlobalOptions"/>
  public AnalyzerConfigOptions GlobalOptions { get; } = globalOptions;

  /// <inheritdoc cref="GeneratorAttributeSyntaxContext.SemanticModel"/>
  public SemanticModel SemanticModel { get; } = syntaxContext.SemanticModel;

  /// <inheritdoc cref="GeneratorAttributeSyntaxContext.TargetNode"/>
  public SyntaxNode TargetNode { get; } = syntaxContext.TargetNode;

  /// <inheritdoc cref="GeneratorAttributeSyntaxContext.TargetSymbol"/>
  public ISymbol TargetSymbol { get; } = syntaxContext.TargetSymbol;
}
