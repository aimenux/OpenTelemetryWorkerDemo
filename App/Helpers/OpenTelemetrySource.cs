using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace App.Helpers;

public sealed class OpenTelemetrySource
{
    public readonly ActivitySource ActivitySource;

    public const string SourceName = "OpenTelemetrySource";

    public Activity? StartActivity([CallerMemberName] string? caller = null) => ActivitySource.StartActivity($"Activity.{caller}");

    private static readonly Lazy<OpenTelemetrySource> Lazy = new(() => new OpenTelemetrySource());

    public static OpenTelemetrySource Instance => Lazy.Value;

    private OpenTelemetrySource()
    {
        ActivitySource = new ActivitySource(SourceName);
    }
}