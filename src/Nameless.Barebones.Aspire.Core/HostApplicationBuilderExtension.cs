using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Nameless.Barebones.Aspire;

public static class HostApplicationBuilderExtension {
    /// <summary>
    /// Registers Aspire services and discovery.
    /// </summary>
    /// <typeparam name="TApplicationBuilder">Type of the application builder.</typeparam>
    /// <param name="self">The current <see cref="IHostApplicationBuilder"/> instance.</param>
    /// <returns>The current <see cref="IHostApplicationBuilder"/> instance so other actions can be chained.</returns>
    public static TApplicationBuilder RegisterAspireServices<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IHostApplicationBuilder
        => self.RegisterOpenTelemetry()
               .RegisterHealthChecks()
               .RegisterDiscoveryServices();

    private static TApplicationBuilder RegisterHealthChecks<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return self;
    }

    private static TApplicationBuilder RegisterOpenTelemetry<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IHostApplicationBuilder {
        self.Logging
            .AddOpenTelemetry(logging => {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

        // To add gRPC instrumentation for OpenTelemetry
        // Check: https://learn.microsoft.com/en-us/aspnet/core/grpc/diagnostics?view=aspnetcore-9.0
        self.Services
            .AddOpenTelemetry()
            .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation()
                                           .AddHttpClientInstrumentation()
                                           .AddRuntimeInstrumentation())
            .WithTracing(tracing => tracing.AddSource(self.Environment.ApplicationName)
                                           .AddAspNetCoreInstrumentation()
                                           .AddHttpClientInstrumentation());

        var configValue = self.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        var useOpenTelemetryExporters = !string.IsNullOrWhiteSpace(configValue);

        if (useOpenTelemetryExporters) {
            self.Services
                .AddOpenTelemetry()
                .UseOtlpExporter();
        }

        return self;
    }

    private static TApplicationBuilder RegisterDiscoveryServices<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddServiceDiscovery()
            .ConfigureHttpClientDefaults(http => {
                http.AddStandardResilienceHandler(); // Turn on resilience by default
                http.AddServiceDiscovery(); // Turn on service discovery by default
            });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return self;
    }
}