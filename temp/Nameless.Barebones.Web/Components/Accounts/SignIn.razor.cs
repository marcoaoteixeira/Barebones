using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;
using Nameless.Barebones.Web.Resources;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class SignIn {
    private readonly SignInManager<User> _signInManager;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<SignIn> _logger;

    private string? StatusMessage { get; set; }

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private IStringLocalizer<SignIn> T { get; }

    public SignIn(SignInManager<User> signInManager,
                  IRedirectManager redirectManager,
                  IStringLocalizer<SignIn> localizer,
                  ILogger<SignIn> logger) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _logger = Prevent.Argument.Null(logger);

        T = Prevent.Argument.Null(localizer);
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
            _redirectManager.Redirect(ReturnUrl);

            return;
        }

        // User need two-factor authentication, redirect to the two-factor page.
        if (result.RequiresTwoFactor) {
            var queryParams = new Dictionary<string, object?> {
                    ["returnUrl"] = ReturnUrl,
                    ["rememberMe"] = Input.RememberMe
                };

            _redirectManager.Redirect(Constants.Urls.Accounts.SignInWithTwoFactor,
                                      queryParams);

            return;
        }

        // User is locked out, redirect to the lockout page.
        if (result.IsLockedOut) {
            _logger.LogWarning("User account locked out.");
            _redirectManager.Redirect(Constants.Urls.Accounts.Lockout);

            return;
        }

        // if we reach here, something failed, display an error
        StatusMessage = "Error: Invalid login attempt.";
    }

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        if (HttpMethods.IsGet(HttpContext.Request.Method)) {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    private sealed record InputModel {
        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
