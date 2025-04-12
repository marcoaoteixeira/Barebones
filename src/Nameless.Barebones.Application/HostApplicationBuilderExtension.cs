using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.WebApplication.Application;

public static class HostApplicationBuilderExtension {
    public static TApplicationBuilder RegisterApplication<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IHostApplicationBuilder
        => self.RegisterUserAccessor()
               .RegisterRedirectManager();

    private static TApplicationBuilder RegisterUserAccessor<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IHostApplicationBuilder {
        self.Services.AddScoped<IUserAccessor, HttpContextUserAccessor>();

        return self;
    }

    private static TApplicationBuilder RegisterRedirectManager<TApplicationBuilder>(this TApplicationBuilder self)
        where TApplicationBuilder : IHostApplicationBuilder {
        self.Services.AddScoped<IRedirectManager, RedirectManager>();

        return self;
    }
}
