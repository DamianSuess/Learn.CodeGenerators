using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MvvmApp.Generators;

/// <summary>Notify property generator Prism.Mvvm.BindableBase property generator.</summary>
/// <remarks>
///   References:
///   - https://themisir.com/c-sharp-codegen/
/// </remarks>
[Generator(LanguageNames.CSharp)]
public class NotifyGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Register attribute source
    context.RegisterPostInitializationOutput(ctx =>
      ctx.AddSource(
        "FileName.g.cs",
        SourceText.From(SourceHelper.NotifyFieldAttributeClass, Encoding.UTF8)));

    // Filter for properties
    // ...
  }
}
