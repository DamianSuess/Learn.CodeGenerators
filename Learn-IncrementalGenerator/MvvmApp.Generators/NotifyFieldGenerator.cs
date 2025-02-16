using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MvvmApp.Generators;

/// <summary>Notify property generator Prism.Mvvm.BindableBase property generator.</summary>
/// <remarks>
///   References:
///   - https://themisir.com/c-sharp-codegen/
/// </remarks>
[Generator(LanguageNames.CSharp)]
public class NotifyFieldGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Register attribute source
    context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
      $"{SourceHelper.NotifyFieldAttribute}.g.cs",
      SourceText.From(SourceHelper.NotifyFieldAttributeClass, Encoding.UTF8)));

    var fieldsProvider = context.SyntaxProvider
      .CreateSyntaxProvider(
        predicate: static (node, _) => IsFieldTargetForGenerator(node),
        transform: GetFieldsToTransform);
    ////.Where(static m => m is not null);

    var fields = fieldsProvider.SelectMany((collection, _) => collection);

    var collected = fields.Collect()
      .Combine(context.CompilationProvider);

    context.RegisterSourceOutput(collected, Execute);

    /*
    // Filter for properties
    IncrementalValuesProvider<PropertiesToGenerate?> propsToGenerate = context.SyntaxProvider
      .CreateSyntaxProvider(
        predicate: static (s, _) => IsFieldTargetForGeneration(s),          // Runs on every key press
        transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))  // Runs for every node selected
      .Where(static m => m is not null);
    */
  }

  /// <summary>Finds fields with (any) attribute(s).</summary>
  /// <param name="node">Syntax Node.</param>
  /// <returns>True when valid.</returns>
  /// <remarks>Same as <code>node is FieldDeclarationSyntax f && f.AttributeLists.Count > 0;</code>.</remarks>
  private static bool IsFieldTargetForGenerator(SyntaxNode node) =>
    node is FieldDeclarationSyntax { AttributeLists.Count: > 0 };

  /// <summary>Execute the source generator.</summary>
  /// <param name="context"><see cref="SourceProductionContext"/>.</param>
  /// <param name="source">Source Tuple.</param>
  private void Execute(SourceProductionContext context, (ImmutableArray<IFieldSymbol> symbols, Compilation compilation) source)
  {
    INamedTypeSymbol attributeSymbol = source.compilation.GetTypeByMetadataName(SourceHelper.NotifyFieldAttributeFullNamespace);
    INamedTypeSymbol notifySymbol = source.compilation.GetTypeByMetadataName(SourceHelper.NotifyPropertyChangedNamespace);

    // Group the fields by class and generate the source
    foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in
      source.symbols.GroupBy<IFieldSymbol, INamedTypeSymbol>(
        f => f.ContainingType, SymbolEqualityComparer.Default))
    {
      string srcClass = SourceHelper.GeneratePropertyClass(context, group.Key, group.ToList(), attributeSymbol, notifySymbol);

      if (srcClass is not null)
        context.AddSource($"{group.Key.Name}_Notifyable.g.cs", SourceText.From(srcClass, Encoding.UTF8));
      else
        context.ReportDiagnostic(Diagnostic.Create(DiagnosticCodes.FailedToParseMessage, location: null));
    }
  }

  /// <summary>Get collection of fields to generate.</summary>
  /// <param name="context">Syntax generator context.</param>
  /// <param name="ct">Cancellation token.</param>
  /// <returns>Collection of fields to transform.</returns>
  private IEnumerable<IFieldSymbol> GetFieldsToTransform(GeneratorSyntaxContext context, CancellationToken ct)
  {
    var syntax = (FieldDeclarationSyntax)context.Node;

    foreach (VariableDeclaratorSyntax variable in syntax.Declaration.Variables)
    {
      IFieldSymbol? fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;

      // if (fieldSymbol is null) { continue; }

      if (fieldSymbol!.GetAttributes().Any(
        ad => ad.AttributeClass!.ToDisplayString() == SourceHelper.NotifyFieldAttributeFullNamespace))
        yield return fieldSymbol;
    }
  }
}
