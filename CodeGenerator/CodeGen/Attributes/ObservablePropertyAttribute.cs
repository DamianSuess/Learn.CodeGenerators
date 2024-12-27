using System;
using System.ComponentModel;

namespace CodeGen.Attributes;

// Ref: https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.Mvvm/ComponentModel/Attributes/ObservablePropertyAttribute.cs

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ObservablePropertyAttribute : Attribute
{
}
