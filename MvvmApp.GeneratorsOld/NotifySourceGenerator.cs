// #define DEBUG_GENERATOR

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MvvmApp.GeneratorsOld;

/// <summary>Notifiable property source code generator.</summary>
/// <remarks>
///   References:
///     - https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/AutoNotifyGenerator.cs
/// </remarks>
[Generator(LanguageNames.CSharp)]
public class NotifySourceGenerator : ISourceGenerator
{
  private const string NotifyFieldAttribute = "NotifyFieldAttribute";

  private const string NotifyFieldAttributeClass = $@"
using System;
namespace {PrismAvaloniaToolkit}
{{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""NotifyFieldGenerator_DEBUG"")]
    sealed class {NotifyFieldAttribute} : Attribute
    {{
        public {NotifyFieldAttribute}()
        {{
        }}

        public string PropertyName {{ get; set; }}
    }}
}}
";

  private const string NotifyFieldNamespace = $"{PrismAvaloniaToolkit}.{NotifyFieldAttribute}";
  private const string NotifyPropertyChanged = "System.ComponentModel.INotifyPropertyChanged";
  private const string PrismAvaloniaToolkit = "Prism.Avalonia.Toolkit";

  public void Execute(GeneratorExecutionContext context)
  {
#if DEBUG_GENERATOR
    while (!System.Diagnostics.Debugger.IsAttached)
      System.Threading.Thread.Sleep(500);
#endif

    // Retrieve the populated receiver
    if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
      return;

    INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName(NotifyFieldNamespace);
    INamedTypeSymbol notifySymbol = context.Compilation.GetTypeByMetadataName(NotifyPropertyChanged);

    // Group the fields by class and generate the source
    foreach (
      IGrouping<INamedTypeSymbol, IFieldSymbol> group in
      receiver.Fields.GroupBy<IFieldSymbol, INamedTypeSymbol>(f => f.ContainingType, SymbolEqualityComparer.Default))
    {
      string srcClass = ProcessClass(group.Key, group.ToList(), attributeSymbol, notifySymbol, context);

      if (srcClass is null)
        continue;

      context.AddSource($"{group.Key.Name}_notifyable.g.cs", SourceText.From(srcClass, Encoding.UTF8));
    }
  }

  public void Initialize(GeneratorInitializationContext context)
  {
    // Register attribute source
    context.RegisterForPostInitialization((i) => i.AddSource($"{NotifyFieldAttribute}.g.cs", NotifyFieldAttributeClass));

    // Register syntax receiver that will be created for each generation pass
    context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
  }

  private string? ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, ISymbol attr, ISymbol notify, GeneratorExecutionContext context)
  {
    // Must be top-level
    if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
      return null;

    string srcNamespace = classSymbol.ContainingNamespace.ToDisplayString();

    // TODO: Use Prism's BindableBase
    StringBuilder src = new($@"
namespace {srcNamespace}
{{
  //// public partial class {classSymbol.Name} : {notify.ToDisplayString()}
  public partial class {classSymbol.Name} : Prism.Mvvm.BindableBase
  {{
");

    // Add the event INotifyPropertyChanged  if it doesnt already
    ////if (!classSymbol.Interfaces.Contains(notify, SymbolEqualityComparer.Default))
    ////  src.Append("    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;");

    foreach (IFieldSymbol fieldSymbol in fields)
      ProcessField(src, fieldSymbol, attr);

    src.Append("  }\n}");

    return src.ToString();
  }

  private void ProcessField(StringBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
  {
    const string PropertyName = "PropertyName";

    // Get name and type of the field
    string fieldName = fieldSymbol.Name;
    ITypeSymbol fieldType = fieldSymbol.Type;

    // Get Notifiable attribute from the field and associated data
    AttributeData attributeData = fieldSymbol
      .GetAttributes()
      .Single(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));

    TypedConstant overridenNameOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == PropertyName).Value;

    // Report diagnostic issues if we can't process the field.
    string propertyName = ExtractName(fieldName, overridenNameOpt);
    if (propertyName.Length == 0 || propertyName == fieldName)
    {
      return;
    }

    source.Append($@"
    public {fieldType} {propertyName}
    {{
      get => this.{fieldName};
      set
      {{
          SetProperty(ref this.{fieldName}, value);
          //this.{fieldName} = value;
          // this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof({propertyName})));
      }}
    }}
");

    // Extract the new Property's name from the field
    string ExtractName(string fieldName, TypedConstant overridenNameOpt)
    {
      if (!overridenNameOpt.IsNull)
      {
        return overridenNameOpt.Value.ToString();
      }

      // Remove leading underscore
      fieldName = fieldName.TrimStart('_');
      if (fieldName.Length == 0)
        return string.Empty;

      if (fieldName.Length == 1)
        return fieldName.ToUpper();

      return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
    }
  }

  private class SyntaxReceiver : ISyntaxContextReceiver
  {
    public List<IFieldSymbol> Fields { get; } = [];

    /// <summary>Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation.</summary>
    /// <param name="context"><see cref="GeneratorSyntaxContext"/>.</param>
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
      // Any field with at least one attribute is checked for property generation
      if (context.Node is FieldDeclarationSyntax syntax &&
          syntax.AttributeLists.Count > 0)
      {
        foreach (VariableDeclaratorSyntax variable in syntax.Declaration.Variables)
        {
          IFieldSymbol? symbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;

          if (symbol is null)
            continue;

          if (symbol.GetAttributes().Any(attr => attr.AttributeClass.ToDisplayString() == NotifyFieldNamespace))
          {
            Fields.Add(symbol);
          }
        }
      }
    }
  }
}
