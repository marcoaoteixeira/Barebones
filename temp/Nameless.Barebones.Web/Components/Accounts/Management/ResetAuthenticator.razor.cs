using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class ResetAuthenticator {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextUserAccessor _userAccessor;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<ResetAuthenticator> _logger;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    public ResetAuthenticator(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IHttpContextUserAccessor userAccessor,
                              IRedirectManager redirectManager,
                              ILogger<ResetAuthenticator> logger) {
        _userManager = Prevent.Argument.Null(userManager);
        _signInManager = Prevent.Argument.Null(signInManager);
        _userAccessor = Prevent.Argument.Null(userAccessor);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _logger = Prevent.Argument.Null(logger);
    }

    private async Task OnSubmitAsync() {
        var user = await _userAccessor.GetCurrentUserAsync(HttpContext);
        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);
        var userId = await _userManager.GetUserIdAsync(user);
        _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", userId);

        await _signInManager.RefreshSignInAsync(user);

        _redirectManager.Redirect(
                                             "Account/Manage/EnableAuthenticator",
                                             "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.",
                                             HttpContext);
    }
}
