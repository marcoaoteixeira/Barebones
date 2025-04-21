using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class ConfirmEmailChange {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IRedirectManager _redirectManager;
    private readonly IStringLocalizer<ConfirmEmailChange> _localizer;

    private string? Message { get; set; }

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromQuery]
    private string? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Email { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    public ConfirmEmailChange(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IRedirectManager redirectManager,
                              IStringLocalizer<ConfirmEmailChange> localizer) {
        _userManager = Prevent.Argument.Null(userManager);
        _signInManager = Prevent.Argument.Null(signInManager);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _localizer = Prevent.Argument.Null(localizer);
    }

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        if (UserId is null || Email is null || Code is null) {
            _redirectManager.Redirect(uri: Constants.Urls.SignIn,
                                      statusMessage: _localizer["Error: Invalid email change confirmation link."],
                                      httpContext: HttpContext);
        }

        var user = await _userManager.FindByIdAsync(UserId);
        if (user is null) {
            Message = _localizer["Unable to find user by its Id"];
            return;
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        var result = await _userManager.ChangeEmailAsync(user, Email, code);
        if (!result.Succeeded) {
            Message = _localizer["Error changing email."];
            return;
        }

        // In our UI email and user name are one and the same, so when we
        // update the email we need to update the user name.
        var setUserNameResult = await _userManager.SetUserNameAsync(user, Email);
        if (!setUserNameResult.Succeeded) {
            Message = _localizer["Error changing user name."];
            return;
        }

        await _signInManager.RefreshSignInAsync(user);
        Message = _localizer["Thank you for confirming your email change."];
    }
}
