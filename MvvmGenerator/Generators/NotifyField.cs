using System;

namespace MvvmApp.Generators;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
internal sealed class NotifyField : Attribute
{
  public NotifyField()
  {
  }

  public string PropertyName { get; set; }
}
