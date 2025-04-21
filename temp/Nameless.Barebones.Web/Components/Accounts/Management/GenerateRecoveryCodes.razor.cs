using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class GenerateRecoveryCodes {
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextUserAccessor _userAccessor;
    private readonly ILogger<GenerateRecoveryCodes> _logger;

    private string? message;
    private User user = default!;
    private IEnumerable<string>? recoveryCodes;

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    public GenerateRecoveryCodes(UserManager<User> userManager,
                                 IHttpContextUserAccessor userAccessor,
                                 ILogger<GenerateRecoveryCodes> logger) {
        _userManager = Prevent.Argument.Null(userManager);
        _userAccessor = Prevent.Argument.Null(userAccessor);
        _logger = Prevent.Argument.Null(logger);
    }

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        user = await _userAccessor.GetCurrentUserAsync(HttpContext);

        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled) {
            throw new InvalidOperationException("Cannot generate recovery codes for user because they do not have 2FA enabled.");
        }
    }

    private async Task OnSubmitAsync() {
        var userId = await _userManager.GetUserIdAsync(user);
        recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        message = "You have generated new recovery codes.";

        _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
    }
}
