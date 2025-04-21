using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class ExternalSignIn {
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;
    private readonly IEmailSender<User> _emailSender;
    private readonly NavigationManager _navigationManager;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<ExternalSignIn> _logger;

    private string? message;
    private ExternalLoginInfo? externalLoginInfo;

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? RemoteError { get; set; }

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    [SupplyParameterFromQuery]
    private string? Action { get; set; }

    private string? ProviderDisplayName => externalLoginInfo?.ProviderDisplayName;

    public ExternalSignIn(SignInManager<User> signInManager,
                          UserManager<User> userManager,
                          IUserStore<User> userStore,
                          IEmailSender<User> emailSender,
                          NavigationManager navigationManager,
                          IRedirectManager redirectManager,
                          ILogger<ExternalSignIn> logger) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _userManager = Prevent.Argument.Null(userManager);
        _userStore = Prevent.Argument.Null(userStore);
        _emailSender = Prevent.Argument.Null(emailSender);
        _navigationManager = Prevent.Argument.Null(navigationManager);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _logger = Prevent.Argument.Null(logger);
    }

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        if (RemoteError is not null) {
            _redirectManager.Redirect("Account/Login", $"Error from external provider: {RemoteError}", HttpContext);
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null) {
            _redirectManager.Redirect("Account/Login", "Error loading external login information.", HttpContext);
        }

        externalLoginInfo = info;

        if (HttpMethods.IsGet(HttpContext.Request.Method)) {
            if (Action == Constants.SignInCallbackAction) {
                await OnLoginCallbackAsync();

                return;
            }

            // We should only reach this page via the login callback,
            // so redirect back to the login page if we get here
            // some other way.
            _redirectManager.Redirect("Account/Login");
        }
    }

    private async Task OnLoginCallbackAsync() {
        Prevent.Argument.Null(HttpContext);

        if (externalLoginInfo is null) {
            _redirectManager.Redirect("Account/Login", "Error loading external login information.", HttpContext);
        }

        // Sign in the user with this external login provider if the
        // user already has a login.
        var result = await _signInManager.ExternalLoginSignInAsync(
            externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.Succeeded) {
            _logger.LogInformation(
                "{Name} logged in with {LoginProvider} provider.",
                externalLoginInfo.Principal.Identity?.Name,
                externalLoginInfo.LoginProvider);
            _redirectManager.Redirect(ReturnUrl);
        } else if (result.IsLockedOut) {
            _redirectManager.Redirect("Account/Lockout");
        }

        // If the user does not have an account, then ask the user to create an account.
        if (externalLoginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Email)) {
            Input.Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
        }
    }

    private async Task OnValidSubmitAsync() {
        Prevent.Argument.Null(HttpContext);

        if (externalLoginInfo is null) {
            _redirectManager.Redirect("Account/Login", "Error loading external login information during confirmation.", HttpContext);
        }

        var emailStore = GetEmailStore();
        var user = CreateUser();

        await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded) {
            result = await _userManager.AddLoginAsync(user, externalLoginInfo);
            if (result.Succeeded) {
                _logger.LogInformation("User created an account using {Name} provider.", externalLoginInfo.LoginProvider);

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = _navigationManager.GetUriWithQueryParameters(_navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                                                                               new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
                await _emailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

                // If account confirmation is required, we need to show the link if we don't have a real email sender
                if (_userManager.Options.SignIn.RequireConfirmedAccount) {
                    _redirectManager.Redirect("Account/RegisterConfirmation", new Dictionary<string, object?> { ["email"] = Input.Email });
                }

                await _signInManager.SignInAsync(user, isPersistent: false, externalLoginInfo.LoginProvider);
                _redirectManager.Redirect(ReturnUrl);
            }
        }

        message = $"Error: {string.Join(",", result.Errors.Select(error => error.Description))}";
    }

    private User CreateUser() {
        try {
            return Activator.CreateInstance<User>();
        } catch {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor");
        }
    }

    private IUserEmailStore<User> GetEmailStore() {
        if (!_userManager.SupportsUserEmail) {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<User>)_userStore;
    }

    private sealed class InputModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
