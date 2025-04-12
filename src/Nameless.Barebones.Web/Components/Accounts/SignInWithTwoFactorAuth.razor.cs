using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class SignInWithTwoFactorAuth {
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<SignInWithTwoFactorAuth> _logger;

    private string? message;
    private User user = default!;

    public SignInWithTwoFactorAuth(SignInManager<User> signInManager, UserManager<User> userManager, IRedirectManager redirectManager, ILogger<SignInWithTwoFactorAuth> logger) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _userManager = Prevent.Argument.Null(userManager);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _logger = Prevent.Argument.Null(logger);
    }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    [SupplyParameterFromQuery]
    private bool RememberMe { get; set; }

    protected override async Task OnInitializedAsync() {
        // Ensure the user has gone through the username & password screen first
        user = await _signInManager.GetTwoFactorAuthenticationUserAsync() ??
               throw new InvalidOperationException("Unable to load two-factor authentication user.");
    }

    private async Task OnValidSubmitAsync() {
        var authenticatorCode = Input.TwoFactorCode!.Replace(" ", string.Empty).Replace("-", string.Empty);
        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, RememberMe, Input.RememberMachine);
        var userId = await _userManager.GetUserIdAsync(user);

        if (result.Succeeded) {
            _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", userId);
            _redirectManager.Redirect(ReturnUrl);
        } else if (result.IsLockedOut) {
            _logger.LogWarning("User with ID '{UserId}' account locked out.", userId);
            _redirectManager.Redirect("Account/Lockout");
        } else {
            _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", userId);
            message = "Error: Invalid authenticator code.";
        }
    }

    private sealed class InputModel {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string? TwoFactorCode { get; set; }

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }
}
