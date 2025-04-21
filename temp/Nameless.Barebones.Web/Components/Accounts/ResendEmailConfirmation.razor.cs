using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class ResendEmailConfirmation {
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender<User> _emailSender;
    private readonly NavigationManager _navigationManager;

    private string? message;

    public ResendEmailConfirmation(UserManager<User> userManager,
                                   IEmailSender<User> emailSender,
                                   NavigationManager navigationManager) {
        _userManager = Prevent.Argument.Null(userManager);
        _emailSender = Prevent.Argument.Null(emailSender);
        _navigationManager = Prevent.Argument.Null(navigationManager);
    }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    private async Task OnValidSubmitAsync() {
        var user = await _userManager.FindByEmailAsync(Input.Email!);
        if (user is null) {
            message = "Verification email sent. Please check your email.";
            return;
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = _navigationManager.GetUriWithQueryParameters(
                                                                      _navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                                                                      new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
        await _emailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        message = "Verification email sent. Please check your email.";
    }

    private sealed class InputModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
