using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Nameless.Barebones.Aspire;

public static class WebApplicationExtension {
    public static WebApplication UseHealthCheckServices(this WebApplication self) {
        // Adding health checks endpoints to applications in non-development
        // environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before
        // enabling these endpoints in non-development environments.
        if (self.Environment.IsDevelopment()) {
            // All health checks must pass for app to be considered ready
            // to accept traffic after starting
            self.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for
            // app to be considered alive
            self.MapHealthChecks("/alive", new HealthCheckOptions {
                Predicate = registration => registration.Tags.Contains("live")
            });
        }

        return self;
    }
}
