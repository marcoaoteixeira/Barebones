using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Nameless.Barebones.Aspire;
using Nameless.Barebones.Domains;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Email;
using Nameless.Barebones.Infrastructure.Identity;
using Nameless.Barebones.Infrastructure.Navigation;
using Nameless.Barebones.Web.Components;
using Nameless.Barebones.Web.Configs;

namespace Nameless.Barebones.Web;

public static class Entrypoint {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // This must be the first thing to be registered
        builder.RegisterAspireServices();

        builder.Services
               .RegisterRazorServices()
               .RegisterIdentityServices()
               .RegisterDatabaseServices(builder.Configuration)
               .RegisterNotificationServices()
               .RegisterLocalizationServices()
               .RegisterNavigationServices()
               .RegisterFastEndpointsServices();

        builder.Build()
               .UseSecurityServices()
               .UseRazorServices()
               .UseFastEndpointsServices()
               .UseHealthCheckServices()
               .Run();
    }
}
