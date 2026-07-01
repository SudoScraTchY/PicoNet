using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace PicoNet.UI.Extensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry for Blazor UI observability
/// </summary>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Add OpenTelemetry tracing, metrics, and logs for Blazor UI application.
    /// Optional configuration for detailed observability with Aspire.
    /// </summary>
    /// <remarks>
    /// This requires OpenTelemetry NuGet packages:
    /// - OpenTelemetry
    /// - OpenTelemetry.Exporter.Otlp
    /// - OpenTelemetry.Instrumentation.Http
    /// - OpenTelemetry.Instrumentation.Runtime
    /// </remarks>
    public static IServiceCollection AddOpenTelemetryInstrumentation(
        this IServiceCollection services,
        WebApplicationBuilder builder,
        bool enableMetrics = true,
        bool enableTracing = true,
        bool enableLogs = true)
    {
        var otelEnabled = builder.Configuration.GetValue<bool>("OpenTelemetry:Enabled", false);
        if (!otelEnabled)
            return services;

        var serviceName = "PicoNet.UI";
        var serviceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(serviceName, serviceVersion: serviceVersion);

        if (enableTracing)
        {
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317");
                            options.Protocol = OtlpExportProtocol.Grpc;
                        });
                });
        }

        if (enableMetrics)
        {
            builder.Services.AddOpenTelemetry()
                .WithMetrics(metricsProviderBuilder =>
                {
                    metricsProviderBuilder
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317");
                            options.Protocol = OtlpExportProtocol.Grpc;
                        });
                });
        }

        if (enableLogs)
        {
            builder.Logging.AddOpenTelemetry(loggingBuilder =>
            {
                loggingBuilder
                    .SetResourceBuilder(resourceBuilder)
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317");
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
            });
        }

        return services;
    }
}
