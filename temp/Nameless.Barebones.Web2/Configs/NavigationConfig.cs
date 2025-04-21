using Nameless.Barebones.Web2.Components.Account;

namespace Nameless.Barebones.Web.Configs;

internal static class NavigationConfig {
    internal static IServiceCollection RegisterNavigationServices(this IServiceCollection self)
        => self.AddScoped<IdentityRedirectManager>();
}
