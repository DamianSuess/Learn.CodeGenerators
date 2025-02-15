using System;

namespace Prism.Avalonia.Toolkit;

[AttributeUsage(
  AttributeTargets.Field,
  Inherited = false,
  AllowMultiple = false)]
public sealed class NotifyFieldAttribute : Attribute
{
  public NotifyFieldAttribute()
  {
  }

  public string PropertyName { get; set; }
}
