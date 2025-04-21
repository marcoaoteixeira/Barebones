using Microsoft.FluentUI.AspNetCore.Components;
using Nameless.Barebones.Web2.Components;

namespace Nameless.Barebones.Web.Configs;

internal static class RazorConfig {
    internal static IServiceCollection RegisterRazorServices(this IServiceCollection self) {
        self.AddRazorComponents()
            .AddInteractiveServerComponents();
        self.AddFluentUIComponents();

        return self;
    }

    internal static WebApplication UseRazorServices(this WebApplication self) {
        self.MapStaticAssets();
        self.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return self;
    }
}
