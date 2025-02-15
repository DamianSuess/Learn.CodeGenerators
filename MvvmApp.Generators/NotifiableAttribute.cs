using System;

namespace Prism.Avalonia.Toolkit;

[AttributeUsage(
  AttributeTargets.Property,
  Inherited = false,
  AllowMultiple = false)]
public sealed class NotifiableAttribute : Attribute
{
  public NotifiableAttribute()
  {
  }

  public string? PropertyName { get; set; }
}
