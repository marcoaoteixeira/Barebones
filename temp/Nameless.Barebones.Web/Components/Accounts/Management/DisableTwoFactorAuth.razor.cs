using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class DisableTwoFactorAuth {
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextUserAccessor _httpContextUserAccessor;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<DisableTwoFactorAuth> _logger;

    private User user = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    public DisableTwoFactorAuth(UserManager<User> userManager,
                                IHttpContextUserAccessor httpContextUserAccessor,
                                IRedirectManager redirectManager,
                                ILogger<DisableTwoFactorAuth> logger) {
        _userManager = Prevent.Argument.Null(userManager);
        _httpContextUserAccessor = Prevent.Argument.Null(httpContextUserAccessor);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _logger = Prevent.Argument.Null(logger);
    }

    protected override async Task OnInitializedAsync() {
        user = await _httpContextUserAccessor.GetCurrentUserAsync(HttpContext);

        if (HttpMethods.IsGet(HttpContext.Request.Method) && !await _userManager.GetTwoFactorEnabledAsync(user)) {
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");
        }
    }

    private async Task OnSubmitAsync() {
        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded) {
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");
        }

        var userId = await _userManager.GetUserIdAsync(user);
        _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", userId);
        _redirectManager.Redirect(
                                             "Account/Manage/TwoFactorAuthentication",
                                             "2fa has been disabled. You can reenable 2fa when you setup an authenticator app",
                                             HttpContext);
    }
}
