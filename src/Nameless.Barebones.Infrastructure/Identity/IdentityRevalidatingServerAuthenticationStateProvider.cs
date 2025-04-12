using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Infrastructure.Identity;

// This is a server-side AuthenticationStateProvider that revalidates
// the security stamp for the connected user every X minutes
// (see RevalidationInterval property) an interactive circuit is connected.
public sealed class IdentityRevalidatingServerAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<IdentityOptions> _options;

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    public IdentityRevalidatingServerAuthenticationStateProvider(ILoggerFactory loggerFactory,
                                                                 IServiceScopeFactory scopeFactory,
                                                                 IOptions<IdentityOptions> options) : base(loggerFactory) {
        _scopeFactory = Prevent.Argument.Null(scopeFactory);
        _options = Prevent.Argument.Null(options);
    }

    protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken) {
        // Get the user manager from a new scope to ensure it fetches fresh data
        await using var scope = _scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider
                               .GetRequiredService<UserManager<User>>();

        return await ValidateSecurityStampAsync(userManager, authenticationState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<User> userManager, ClaimsPrincipal principal) {
        var user = await userManager.GetUserAsync(principal);

        if (user is null) {
            return false;
        }

        if (!userManager.SupportsUserSecurityStamp) {
            return true;
        }

        var claimType = _options.Value.ClaimsIdentity.SecurityStampClaimType;
        var principalSecurityStamp = principal.FindFirstValue(claimType);
        var userSecurityStamp = await userManager.GetSecurityStampAsync(user);

        return principalSecurityStamp == userSecurityStamp;
    }
}
