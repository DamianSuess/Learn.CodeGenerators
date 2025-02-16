/*
// This file aims to optimize the previous generator

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MvvmApp.Generators;

/// <summary>Notify property generator Prism.Mvvm.BindableBase property generator.</summary>
/// <remarks>
///   References:
///   - https://themisir.com/c-sharp-codegen/
/// </remarks>
////[Generator(LanguageNames.CSharp)]
public class NotifyGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Register attribute source
    context.RegisterPostInitializationOutput(ctx =>
      ctx.AddSource(
        $"{SourceHelper.NotifyFieldAttribute}.g.cs",
        SourceText.From(SourceHelper.NotifyFieldAttributeClass, Encoding.UTF8)));

    // Filter for properties
    IncrementalValuesProvider<PropertiesToGenerate?> propsToGenerate = context.SyntaxProvider
      .CreateSyntaxProvider(
          predicate: static (s, _) => IsFieldTargetForGeneration(s),          // Runs on every key press
          transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))  // Runs for every node selected
      .Where(static m => m is not null);

    // More efficient finder (.NET 7+)
    /*
    IncrementalValuesProvider<PropertiesToGenerate?> propsToGenerate7 = context.SyntaxProvider
      .ForAttributeWithMetadataName(
        SourceHelper.NotifyFieldAttributeFullNamespace,
        predicate: static (node, _) => node is FieldDeclarationSyntax f && f.AttributeLists.Count > 0,
        transform: static (ctx, _) => GetPropertiesToGenerate(ctx.SemanticModel, ctx.TargetNode))
      .Where(static m => m is not null);
    *-/

    // Generate the source code
    context.RegisterSourceOutput(propsToGenerate, static (spc, source)
      => Execute(source, spc));
  }

  private static void Execute(PropertiesToGenerate? properties, SourceProductionContext context)
  {
    /*
    if (properties is { } value)
    {
      //// string srcClass = SourceHelper.GeneratePropertyClass(group.Key, group.ToList(), attributeSymbol, notifySymbol);
      string result = SourceHelper.GeneratePropertyClass(value);

      //// $"{value.Name}_Notifyable.g.cs"

      if (result is not null)
        context.AddSource($"PropertyExtension.{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
    }
    *-/
  }

  private static PropertiesToGenerate? GetPropertiesToGenerate(SemanticModel semanticModel, SyntaxNode targetNode)
  {
    // Get representation of syntax
    if (semanticModel.GetDeclaredSymbol(targetNode) is not IFieldSymbol fieldSymbol) //// INamedTypeSymbol fieldSymbol)
    {
      // TODO: Report diagnostic error
      return null;
    }

    // Get the full type name (enumerations use: GetMembers())
    string fieldName = fieldSymbol.ToString();
    ImmutableArray<AttributeData> fieldMembers = fieldSymbol.GetAttributes();

    /*
    var fields = new List<string>();

    foreach (var attr in fieldMembers)
    {
      if (attr.AttributeClass.ToDisplayString() == SourceHelper.NotifyFieldAttributeFullNamespace)
        fields.Add(fieldSymbol);
    }
    *-/

    return null;
  }

  /// <summary>Select fields with the [EnumExtensions] attribute and extract details.</summary>
  /// <param name="ctx"></param>
  /// <returns></returns>
  private static PropertiesToGenerate? GetSemanticTargetForGeneration(GeneratorSyntaxContext ctx)
  {
    // Pre .NET7 method
    return new();
  }

  /// <summary>Finds fields with (any) attribute(s).</summary>
  /// <param name="node">Syntax Node.</param>
  /// <returns>True when valid.</returns>
  /// <remarks>Same as <code>node is FieldDeclarationSyntax f && f.AttributeLists.Count > 0;</code>.</remarks>
  private static bool IsFieldTargetForGeneration(SyntaxNode node) =>
    node is FieldDeclarationSyntax { AttributeLists.Count: > 0 };

  public readonly record struct PropertiesToGenerate
  {
    public readonly string Name;
    public readonly List<string> Values;

    public PropertiesToGenerate(string name, List<string> values)
    {
      Name = name;
      Values = values;
    }
  }
}
*/
