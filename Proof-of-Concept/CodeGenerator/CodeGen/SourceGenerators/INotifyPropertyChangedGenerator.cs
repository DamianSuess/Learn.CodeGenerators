﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGen.Helpers;
using CodeGen.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace CodeGen.SourceGenerators;

/// <summary>
/// A source generator for the <c>INotifyPropertyChangedAttribute</c> type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class INotifyPropertyChangedGenerator : TransitiveMembersGenerator<INotifyPropertyChangedInfo>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="INotifyPropertyChangedGenerator"/> class.
  /// </summary>
  public INotifyPropertyChangedGenerator()
      : base("CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute")
  {
  }

  /// <inheritdoc/>
  private protected override INotifyPropertyChangedInfo? ValidateTargetTypeAndGetInfo(INamedTypeSymbol typeSymbol, AttributeData attributeData, Compilation compilation, out ImmutableArray<DiagnosticInfo> diagnostics)
  {
    diagnostics = ImmutableArray<DiagnosticInfo>.Empty;

    INotifyPropertyChangedInfo? info = null;

    // Check if the type already implements INotifyPropertyChanged
    if (typeSymbol.AllInterfaces.Any(i => i.HasFullyQualifiedMetadataName("System.ComponentModel.INotifyPropertyChanged")))
    {
      diagnostics = ImmutableArray.Create(DiagnosticInfo.Create(DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError, typeSymbol, typeSymbol));

      goto End;
    }

    // Check if the type uses [INotifyPropertyChanged] or [ObservableObject] already (in the type hierarchy too)
    if (typeSymbol.HasOrInheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.ObservableObjectAttribute") ||
        typeSymbol.InheritsAttributeWithFullyQualifiedMetadataName("CommunityToolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute"))
    {
      diagnostics = ImmutableArray.Create(DiagnosticInfo.Create(InvalidAttributeCombinationForINotifyPropertyChangedAttributeError, typeSymbol, typeSymbol));

      goto End;
    }

    bool includeAdditionalHelperMethods = attributeData.GetNamedArgument("IncludeAdditionalHelperMethods", true);

    info = new INotifyPropertyChangedInfo(includeAdditionalHelperMethods);

  End:
    return info;
  }

  /// <inheritdoc/>
  protected override ImmutableArray<MemberDeclarationSyntax> FilterDeclaredMembers(INotifyPropertyChangedInfo info, ImmutableArray<MemberDeclarationSyntax> memberDeclarations)
  {
    // If requested, only include the event and the basic methods to raise it, but not the additional helpers
    if (!info.IncludeAdditionalHelperMethods)
    {
      using ImmutableArrayBuilder<MemberDeclarationSyntax> selectedMembers = ImmutableArrayBuilder<MemberDeclarationSyntax>.Rent();

      foreach (MemberDeclarationSyntax memberDeclaration in memberDeclarations)
      {
        if (memberDeclaration is EventFieldDeclarationSyntax or MethodDeclarationSyntax { Identifier.ValueText: "OnPropertyChanged" })
        {
          selectedMembers.Add(memberDeclaration);
        }
      }

      return selectedMembers.ToImmutable();
    }

    return memberDeclarations;
  }
}
