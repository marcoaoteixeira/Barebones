using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Web2.Components.Account;
using Nameless.Barebones.Web2.Data;

namespace Nameless.Barebones.Web.Configs;

internal static class NotificationConfig {
    internal static IServiceCollection RegisterNotificationServices(this IServiceCollection self) {
        self.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        return self;
    }
}
