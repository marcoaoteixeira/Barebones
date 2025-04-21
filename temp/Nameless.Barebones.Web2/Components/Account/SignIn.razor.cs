using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Nameless.Barebones.Web2.Data;

namespace Nameless.Barebones.Web2.Components.Account;

public partial class SignIn {
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IdentityRedirectManager _redirectManager;
    private readonly ILogger<SignIn> _logger;

    private string? StatusMessage { get; set; }

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private IStringLocalizer<SignIn> T { get; }

    public SignIn(SignInManager<ApplicationUser> signInManager,
                  IdentityRedirectManager redirectManager,
                  IStringLocalizer<SignIn> localizer,
                  ILogger<SignIn> logger) {
        _signInManager = signInManager;
        _redirectManager = redirectManager;
        _logger = logger;

        T = localizer;
    }

    public async Task SignInUser() {
        // This does count login failures towards account lockout.
        // To disable password failures to trigger account lockout,
        // set lockoutOnFailure: false
        var result = await _signInManager.PasswordSignInAsync(Input.Email,
                                                              Input.Password,
                                                              Input.RememberMe,
                                                              lockoutOnFailure: true);

        // Sign in succeeded so, return to the specified URL
        if (result.Succeeded) {
            _redirectManager.RedirectTo(ReturnUrl);

            return;
        }

        // User need two-factor authentication, redirect to the two-factor page.
        if (result.RequiresTwoFactor) {
            var queryParams = new Dictionary<string, object?> {
                    ["returnUrl"] = ReturnUrl,
                    ["rememberMe"] = Input.RememberMe
                };

            _redirectManager.RedirectTo("Account/LoginWith2fa",
                                        queryParams);

            return;
        }

        // User is locked out, redirect to the lockout page.
        if (result.IsLockedOut) {
            _logger.LogWarning("User account locked out.");
            _redirectManager.RedirectTo("Account/Lockout");

            return;
        }

        // if we reach here, something failed, display an error
        StatusMessage = "Error: Invalid login attempt.";
    }

    protected override async Task OnInitializedAsync() {
        if (HttpContext is null) { throw new InvalidOperationException(); }

        if (HttpMethods.IsGet(HttpContext.Request.Method)) {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    private sealed record InputModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
