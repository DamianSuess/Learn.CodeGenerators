# Learn .NET Code Generators

Basic examples using .NET Code Generators.

There are two key types of .NET code generators available, **[Incremental Generator](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iincrementalgenerator?view=roslyn-dotnet-4.9.0)** and the deprecated **[Source Generator](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isourcegenerator?view=roslyn-dotnet-4.9.0)**. As this repository grows, both examples will be provided. Focusing primarially on the preferred method. For more information, check out the [Roslyn SDK](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/).

Since these methods have a lack of clear documentation, this article references various sources found on the internet.

The examples provided use [Avalonia](https://avaloniaui.net/) and the [Prism.Avalonia](https://github.com/AvaloniaCommunity/Prism.Avalonia) library for a visual cross-platform MVVM experience.  Sure, a console app would quickly suffice; visuals are sometimes better.

### Forenote

> When creating a code generator, be VERY mindful of your technique. Changes to your code will put pressure on the compiler and can have a negative impact on your developer (IDE) experience. The code samples here don't always use the best techniques.

## Examples

### Incremental Generator Example

_Currently under construction_

### Source Generator Example

Basic example for generating properties based on the attribute `NotifyField` used by an MVVM application.

This project relies on techniques used by the [Prism.Avalonia](https://github.com/AvaloniaCommunity/Prism.Avalonia).

```cs
public partial class MainWindowViewModel
{
  [NotifyField]
  private string _firstName;

  [NotifyField]
  private string _lastName;

  // ...
}
```

How to create custom attributes and code generators

## Getting Started

Your code generator project must be itw own project, and using `netstandard2.0`.

Referencing your code gen project can be done in one of two ways

```xml
<!-- Path to the DLL -->
<Analyzer Include="..\MvvmApp.Generators\bin\Debug\netstandard2.0\MvvmApp.Generators.dll"/>

<!-- Or, reference the project -->
<ProjectReference Include="..\MvvmApp.Generators\MvvmApp.Generators.csproj"  OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
```

### References

* [Source Generators Samples](https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/AutoNotifyGenerator.cs#L50)
* [Incremental Generator](https://andrewlock.net/creating-a-source-generator-part-9-avoiding-performance-pitfalls-in-incremental-generators/)
* Sections of this is based on CommunityToolkit.Mvvm `ObservableProperty`.
