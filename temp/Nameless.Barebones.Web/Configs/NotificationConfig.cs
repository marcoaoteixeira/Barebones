using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Email;

namespace Nameless.Barebones.Web.Configs;

internal static class NotificationConfig {
    internal static IServiceCollection RegisterNotificationServices(this IServiceCollection self) {
        self.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

        return self;
    }
}
