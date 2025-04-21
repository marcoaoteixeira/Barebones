using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class Email {
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender<User> _emailSender;
    private readonly IHttpContextUserAccessor _userAccessor;
    private readonly NavigationManager _navigationManager;

    private string? message;
    private User user = default!;
    private string? email;
    private bool isEmailConfirmed;

    public Email(UserManager<User> userManager,
                 IEmailSender<User> emailSender,
                 IHttpContextUserAccessor userAccessor,
                 NavigationManager navigationManager) {
        _userManager = Prevent.Argument.Null(userManager);
        _emailSender = Prevent.Argument.Null(emailSender);
        _userAccessor = Prevent.Argument.Null(userAccessor);
        _navigationManager = Prevent.Argument.Null(navigationManager);
    }

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm(FormName = "change-email")]
    private InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync() {
        user = await _userAccessor.GetCurrentUserAsync(HttpContext);
        email = await _userManager.GetEmailAsync(user);
        isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

        Input.NewEmail ??= email;
    }

    private async Task OnValidSubmitAsync() {
        if (Input.NewEmail is null || Input.NewEmail == email) {
            message = "Your email is unchanged.";
            return;
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = _navigationManager.GetUriWithQueryParameters(
            _navigationManager.ToAbsoluteUri("Account/ConfirmEmailChange").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["email"] = Input.NewEmail, ["code"] = code });

        await _emailSender.SendConfirmationLinkAsync(user, Input.NewEmail, HtmlEncoder.Default.Encode(callbackUrl));

        message = "Confirmation link to change email sent. Please check your email.";
    }

    private async Task OnSendEmailVerificationAsync() {
        if (email is null) {
            return;
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = _navigationManager.GetUriWithQueryParameters(
            _navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });

        await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));

        message = "Verification email sent. Please check your email.";
    }

    private sealed class InputModel {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string? NewEmail { get; set; }
    }
}
