using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nameless.Barebones.Domains;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;

namespace Nameless.Barebones.Web.Configs;

internal static class IdentityConfig {
    internal static IServiceCollection RegisterIdentityServices(this IServiceCollection self) {
        self.AddCascadingAuthenticationState()
            .AddScoped<HttpContextUserAccessor>()
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingServerAuthenticationStateProvider>()
            .AddAuthentication(options => {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        self.AddIdentityCore<User>(options => {
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return self;
    }

    internal static WebApplication UseIdentityServices(this WebApplication self) {
        // Add additional endpoints required by
        // the Identity /Account Razor components.
        self.MapAdditionalIdentityEndpoints();

        return self;
    }

    // These endpoints are required by the Identity Razor components
    // defined in the /Components/Identity/ directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints) {
        Prevent.Argument.Null(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/PerformExternalLogin", (
            HttpContext context,
            [FromServices] SignInManager<User> signInManager,
            [FromForm] string provider,
            [FromForm] string returnUrl) => {
                IEnumerable<KeyValuePair<string, StringValues>> query = [
                    new("ReturnUrl", returnUrl),
                new("Action", Constants.SignInCallbackAction)];

                var redirectUrl = UriHelper.BuildRelative(
                    context.Request.PathBase,
                    "/Account/ExternalLogin",
                    QueryString.Create(query));

                provider = TemporaryFluentButtonFix(provider);

                var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return TypedResults.Challenge(properties, [provider]);
            });

        accountGroup.MapPost("/Logout", async (
            ClaimsPrincipal user,
            [FromServices] SignInManager<User> signInManager,
            [FromForm] string returnUrl) => {
                await signInManager.SignOutAsync();
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            });

        var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        manageGroup.MapPost("/LinkExternalLogin", async (
            HttpContext context,
            [FromServices] SignInManager<User> signInManager,
            [FromForm] string provider) => {
                // Clear the existing external cookie to ensure a clean login process
                await context.SignOutAsync(IdentityConstants.ExternalScheme);

                var redirectUrl = UriHelper.BuildRelative(
                    context.Request.PathBase,
                    "/Account/Manage/ExternalLogins",
                    QueryString.Create("Action", Constants.LinkSignInCallbackAction));

                provider = TemporaryFluentButtonFix(provider);

                var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
                return TypedResults.Challenge(properties, [provider]);
            });

        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

        manageGroup.MapPost("/DownloadPersonalData", async (
            HttpContext context,
            [FromServices] UserManager<User> userManager,
            [FromServices] AuthenticationStateProvider authenticationStateProvider) => {
                var user = await userManager.GetUserAsync(context.User);
                if (user is null) {
                    return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
                }

                var userId = await userManager.GetUserIdAsync(user);
                downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

                // Only include personal data for download
                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(User).GetProperties().Where(
                                                                           prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps) {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await userManager.GetLoginsAsync(user);
                foreach (var l in logins) {
                    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                }

                personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
                var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

                context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
                return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
            });

        return accountGroup;
    }

    private static string TemporaryFluentButtonFix(string provider) {
        // Temporary workaround for FluentButton returning a provider value twice
        // Split the comma-separated list of strings
        var providers = provider.Split(',');

        // Find the value that appears twice in the list
        provider = providers.GroupBy(p => p)
                            .Where(g => g.Count() == 2)
                            .Select(g => g.Key)
                            .First();
        return provider;
    }
}
