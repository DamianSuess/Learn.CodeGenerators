using Microsoft.CodeAnalysis;

namespace MvvmApp.Generators;

public static class DiagnosticCodes
{
  public static readonly DiagnosticDescriptor FailedToParseMessage = new(
    id: "SAMPLE001",
    title: "Message parser failed",
    messageFormat: "Failed to parse message type '{0}'",
    category: "Parser",
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor MustBeTopLevel = new(
    id: "SAMPLE002",
    title: "Message parser failed",
    messageFormat: "Failed to parse field. Must be top-level '{0}'",
    category: "Parser",
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true);
}
