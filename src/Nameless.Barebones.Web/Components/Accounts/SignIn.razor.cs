using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class SignIn {
    private readonly SignInManager<User> _signInManager;
    private readonly NavigationManager _navigationManager;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<SignIn> _logger;

    private string? errorMessage;

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    public SignIn(SignInManager<User> signInManager,
                  NavigationManager navigationManager,
                  IRedirectManager redirectManager,
                  ILogger<SignIn> logger) {
        _signInManager = signInManager;
        _navigationManager = navigationManager;
        _redirectManager = redirectManager;
        _logger = logger;
    }

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        if (HttpMethods.IsGet(HttpContext.Request.Method)) {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }
    }

    public async Task LoginUser() {
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded) {
            _logger.LogInformation("User logged in.");
            _redirectManager.Redirect(ReturnUrl);
        } else if (result.RequiresTwoFactor) {
            _redirectManager.Redirect(
                                      "Account/LoginWith2fa",
                                      new Dictionary<string, object?> { ["returnUrl"] = ReturnUrl, ["rememberMe"] = Input.RememberMe });
        } else if (result.IsLockedOut) {
            _logger.LogWarning("User account locked out.");
            _redirectManager.Redirect("Account/Lockout");
        } else {
            errorMessage = "Error: Invalid login attempt.";
        }
    }

    private sealed class InputModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
