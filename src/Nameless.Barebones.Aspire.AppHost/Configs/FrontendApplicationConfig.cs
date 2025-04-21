using Projects;

namespace Nameless.Barebones.Aspire.AppHost.Configs;
internal static class FrontendApplicationConfig {
    internal static IDistributedApplicationBuilder RegisterFrontendApplication(this IDistributedApplicationBuilder self) {
        self.AddProject<Nameless_Barebones_Web>("frontend");

        return self;
    }
}
