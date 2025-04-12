using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class SignInWithRecoveryCode {
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<SignInWithRecoveryCode> _logger;

    private string? message;
    private User user = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    public SignInWithRecoveryCode(SignInManager<User> signInManager,
                                  UserManager<User> userManager,
                                  IRedirectManager redirectManager,
                                  ILogger<SignInWithRecoveryCode> logger) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _userManager = Prevent.Argument.Null(userManager);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _logger = Prevent.Argument.Null(logger);
    }

    protected override async Task OnInitializedAsync() {
        // Ensure the user has gone through the username & password screen first
        user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ??
               throw new InvalidOperationException("Unable to load two-factor authentication user.");
    }

    private async Task OnValidSubmitAsync() {
        var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        var userId = await _userManager.GetUserIdAsync(user);

        if (result.Succeeded) {
            _logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", userId);
            _redirectManager.Redirect(ReturnUrl);
        } else if (result.IsLockedOut) {
            _logger.LogWarning("User account locked out.");
            _redirectManager.Redirect("Account/Lockout");
        } else {
            _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", userId);
            message = "Error: Invalid recovery code entered.";
        }
    }

    private sealed class InputModel {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; } = "";
    }
}
