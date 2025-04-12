using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class ForgotPassword {
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender<User> _emailSender;
    private readonly NavigationManager _navigationManager;
    private readonly IRedirectManager _redirectManager;

    public ForgotPassword(UserManager<User> userManager,
                          IEmailSender<User> emailSender,
                          NavigationManager navigationManager,
                          IRedirectManager redirectManager) {
        _userManager = userManager;
        _emailSender = emailSender;
        _navigationManager = navigationManager;
        _redirectManager = redirectManager;
    }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    private async Task OnValidSubmitAsync() {
        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user)) {
            // Don't reveal that the user does not exist or is not confirmed
            _redirectManager.Redirect("Account/ForgotPasswordConfirmation");
        }

        // For more information on how to enable account confirmation and password
        // reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = _navigationManager.GetUriWithQueryParameters(_navigationManager.ToAbsoluteUri("Account/ResetPassword").AbsoluteUri,
                                                                       new Dictionary<string, object?> { ["code"] = code });

        await _emailSender.SendPasswordResetLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        _redirectManager.Redirect("Account/ForgotPasswordConfirmation");
    }

    private sealed class InputModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
