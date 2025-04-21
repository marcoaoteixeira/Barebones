using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Nameless.Barebones.Aspire;
using Nameless.Barebones.Domains;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Web.Components;
using Nameless.Barebones.Web.Components.Accounts;
using Nameless.Barebones.Web.Configs;

namespace Nameless.Barebones.Web;
public class Entrypoint {
    public static void Main(string[] args) {
        //var builder = WebApplication.CreateBuilder(args);

        //// Add services to the container.
        //builder.Services.AddRazorComponents()
        //    .AddInteractiveServerComponents();
        //builder.Services.AddFluentUIComponents();

        //builder.Services.AddLocalization();

        //builder.Services.AddCascadingAuthenticationState();
        //builder.Services.AddScoped<IdentityUserAccessor>();
        //builder.Services.AddScoped<IdentityRedirectManager>();
        //builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        //builder.Services.AddAuthentication(options => {
        //    options.DefaultScheme = IdentityConstants.ApplicationScheme;
        //    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        //})
        //    .AddIdentityCookies();

        //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        //builder.Services.AddDbContext<ApplicationDbContext>(options =>
        //    options.UseSqlite(connectionString));
        //builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        //builder.Services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
        //    .AddEntityFrameworkStores<ApplicationDbContext>()
        //    .AddSignInManager()
        //    .AddDefaultTokenProviders();

        //builder.Services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

        //var app = builder.Build();

        //// Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment()) {
        //    app.UseMigrationsEndPoint();
        //} else {
        //    app.UseExceptionHandler("/Error");
        //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //    app.UseHsts();
        //}

        //app.UseHttpsRedirection();

        //app.UseAntiforgery();

        //app.MapStaticAssets();
        //app.MapRazorComponents<App>()
        //    .AddInteractiveServerRenderMode();

        //// Add additional endpoints required by the Identity /Account Razor components.
        //app.MapAdditionalIdentityEndpoints();

        //app.Run();

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
