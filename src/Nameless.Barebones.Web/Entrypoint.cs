using Nameless.Barebones.Web.Configs;

namespace Nameless.Barebones.Web;

public static class Entrypoint {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
               .RegisterRazorServices()
               .RegisterFastEndpointsServices()
               .RegisterIdentityServices()
               .RegisterDatabaseServices(builder.Configuration)
               .RegisterNotificationServices()
               .RegisterLocalizationServices()
               .RegisterNavigationServices();

        builder.Build()
               .UseSecurityServices()
               .UseRazorServices()
               .UseFastEndpointsServices()
               .Run();
    }
}
