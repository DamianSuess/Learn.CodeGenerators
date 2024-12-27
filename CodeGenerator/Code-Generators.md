# Code Generators

Sample applications using C# Source Generators to create a custom property attribute that is to be used in MVVM applications to create a private field of the same name, and call `NotifyPropertyChanged.

In this example we'll be piecing apart the CommunityToolkit.Mvvm's ObservableProperty attribute, as this is a well-known open source feature.

## Features

* ObservableProperty attribute
* Triggers for other properties
* `PropertyNameCollisionObservablePropertyAttributeAnalyzer`
  * A diagnostic analyzer that generates an error when a generated property from <c>[ObservableProperty]</c> would collide with the field name.
* `ObservablePropertyAttributeWithSupportedTargetDiagnosticSuppressor`
* `InvalidTargetObservablePropertyAttributeAnalyzer`
* `NotifyDataErrorInfoAttribute`

## References

* https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/
* https://stackoverflow.com/questions/64926889/generate-code-for-classes-with-an-attribute
* https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.Mvvm/ComponentModel/Attributes/ObservablePropertyAttribute.cs
* https://github.com/CommunityToolkit/Maui/blob/8.0.1/samples/CommunityToolkit.Maui.Sample/ViewModels/Essentials/FileSaverViewModel.cs

## Other Features

```cs
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ObservablePropertyAttributeWithSupportedTargetDiagnosticSuppressor : DiagnosticSuppressor
{
    /// <inheritdoc/>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(PropertyAttributeListForObservablePropertyField, PropertyAttributeListForObservablePropertyFieldAccessors);

    /// <inheritdoc/>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            SyntaxNode? syntaxNode = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken).FindNode(diagnostic.Location.SourceSpan);

            // Check that the target is effectively [property:] over a field declaration with at least one variable, which is the only case we are interested in
            if (syntaxNode is AttributeTargetSpecifierSyntax { Parent.Parent: FieldDeclarationSyntax { Declaration.Variables.Count: > 0 } fieldDeclaration } attributeTarget &&
                (attributeTarget.Identifier.IsKind(SyntaxKind.PropertyKeyword) || attributeTarget.Identifier.IsKind(SyntaxKind.GetKeyword) || attributeTarget.Identifier.IsKind(SyntaxKind.SetKeyword)))
            {
                SemanticModel semanticModel = context.GetSemanticModel(syntaxNode.SyntaxTree);

                // Get the field symbol from the first variable declaration
                ISymbol? declaredSymbol = semanticModel.GetDeclaredSymbol(fieldDeclaration.Declaration.Variables[0], context.CancellationToken);

                // Check if the field is using [ObservableProperty], in which case we should suppress the warning
                if (declaredSymbol is IFieldSymbol fieldSymbol &&
                    semanticModel.Compilation.GetTypeByMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") is INamedTypeSymbol observablePropertySymbol &&
                    fieldSymbol.HasAttributeWithType(observablePropertySymbol))
                {
                    // Emit the right suppression based on the attribute modifier. For 'property:', Roslyn
                    // will emit the 'CS0657' warning, whereas for 'get:' or 'set:', it will emit 'CS0658'.
                    if (attributeTarget.Identifier.IsKind(SyntaxKind.PropertyKeyword))
                    {
                        context.ReportSuppression(Suppression.Create(PropertyAttributeListForObservablePropertyField, diagnostic));
                    }
                    else
                    {
                        context.ReportSuppression(Suppression.Create(PropertyAttributeListForObservablePropertyFieldAccessors, diagnostic));
                    }
                }
            }
        }
    }
}
```
