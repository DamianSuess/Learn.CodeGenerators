using Microsoft.CodeAnalysis;

namespace MvvmApp.Generators;

internal static class DiagnosticDescriptors
{
  public static readonly DiagnosticDescriptor FailedToParseMessage = new(
    "SAMPLE001",
    "Message parser failed",
    "Failed to parse message type '{0}'",
    "Parser",
    DiagnosticSeverity.Error, true);
}
