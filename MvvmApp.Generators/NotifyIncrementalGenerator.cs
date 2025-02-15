using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace MvvmApp.Generators;

/// <summary>Sample property Prism.Mvvm.BindableBase property generator.</summary>
/// <remarks>
///   References:
///   - https://andrewlock.net/creating-a-source-generator-part-9-avoiding-performance-pitfalls-in-incremental-generators/
///   - https://themisir.com/c-sharp-codegen/
/// </remarks>
[Generator(LanguageNames.CSharp)]
public class NotifyIncrementalGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Register attribute source
    //context.RegisterPostInitializationOutput(ctx =>
    //  ctx.AddSource("FileName.g.cs", SourceText.From(NotifyFieldAttributeClass, Encoding.UTF8)));
  }
}
