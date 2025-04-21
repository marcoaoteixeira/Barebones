using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Configs;

internal static class NavigationConfig {
    internal static IServiceCollection RegisterNavigationServices(this IServiceCollection self)
        => self.AddScoped<IRedirectManager, RedirectManager>();
}
