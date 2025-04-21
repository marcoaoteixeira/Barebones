using FastEndpoints;
using FastEndpoints.Swagger;

namespace Nameless.Barebones.Web.Configs;

internal static class FastEndpointsConfig {
    internal static IServiceCollection RegisterFastEndpointsServices(this IServiceCollection self)
        => self.SwaggerDocument(options => {
                   options.MaxEndpointVersion = 1;
                   options.DocumentSettings = docSettings => {
                       docSettings.DocumentName = "v1";
                       docSettings.Title = Constants.ApplicationName;
                       docSettings.Version = "v1";
                   };
               })
               .AddFastEndpoints();

    internal static WebApplication UseFastEndpointsServices(this WebApplication self) {
        if (self.Environment.IsDevelopment()) {
            self.UseSwaggerGen();
        }

        self.UseFastEndpoints(config => {
            config.Versioning.DefaultVersion = 1;
            config.Versioning.Prefix = "v";
            config.Versioning.PrependToRoute = true;
        });

        return self;
    }
}
