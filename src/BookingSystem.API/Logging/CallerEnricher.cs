using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;
using System.Reflection;

namespace BookingSystem.API.Logging;

/// <summary>
/// Enriches log events with the calling method name and full caller (Type.Method)
/// so logs can be traced to the exact layer and method (e.g. repo vs service).
/// </summary>
public class CallerEnricher : ILogEventEnricher
{
    private const int MaxFramesToScan = 15;
    private static readonly string[] IgnoredNamespaces =
    {
        "Serilog.", "Microsoft.Extensions.Logging.", "Microsoft.AspNetCore.",
        "BookingSystem.API.Logging." // Skip this enricher
    };

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var (methodName, caller) = GetCallerFromStack();
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("MethodName", methodName ?? ""));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Caller", caller ?? ""));
    }

    private static (string? MethodName, string? Caller) GetCallerFromStack()
    {
        try
        {
            var stack = new StackTrace(1, false);
            for (var i = 0; i < Math.Min(stack.FrameCount, MaxFramesToScan); i++)
            {
                var frame = stack.GetFrame(i);
                var method = frame?.GetMethod();
                if (method == null) continue;

                var declaringType = method.DeclaringType;
                var ns = declaringType?.Namespace ?? "";
                if (IgnoredNamespaces.Any(ignored => ns.StartsWith(ignored, StringComparison.Ordinal)))
                    continue;

                var methodName = method.Name;
                var caller = declaringType != null
                    ? $"{declaringType.FullName}.{methodName}"
                    : methodName;
                return (methodName, caller);
            }
        }
        catch
        {
            // Ignore; enricher must not throw
        }

        return (null, null);
    }
}
