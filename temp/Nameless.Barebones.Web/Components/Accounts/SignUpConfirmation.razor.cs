using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Email;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class SignUpConfirmation {
    private readonly UserManager<User> _userManager;
    private readonly IEmailSender<User> _emailSender;
    private readonly NavigationManager _navigationManager;
    private readonly IRedirectManager _redirectManager;
    private readonly IStringLocalizer<SignUpConfirmation> _localizer;

    private string? EmailConfirmationLink { get; set; }
    private string? StatusMessage { get; set; }

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromQuery]
    private string? Email { get; set; }

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    public SignUpConfirmation(UserManager<User> userManager,
                              IEmailSender<User> emailSender,
                              NavigationManager navigationManager,
                              IRedirectManager redirectManager,
                              IStringLocalizer<SignUpConfirmation> localizer) {
        _userManager = Prevent.Argument.Null(userManager);
        _emailSender = Prevent.Argument.Null(emailSender);
        _navigationManager = Prevent.Argument.Null(navigationManager);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _localizer = Prevent.Argument.Null(localizer);
    }

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        if (Email is null) {
            _redirectManager.Redirect(Constants.Urls.Home);
        }

        var user = await _userManager.FindByEmailAsync(Email);

        if (user is null) {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            StatusMessage = _localizer["Error finding user for unspecified email"];

            return;
        }
        
        if (_emailSender is IdentityNoOpEmailSender) {
            // Once you add a real email sender, you should remove this code
            // that lets you confirm the account
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var confirmEmailUri = _navigationManager.ToAbsoluteUri(Constants.Urls.Accounts.ConfirmEmail)
                                                    .AbsoluteUri;
            var parameters = new Dictionary<string, object?> {
                ["userId"] = userId,
                ["code"] = code,
                ["returnUrl"] = ReturnUrl
            };
            EmailConfirmationLink = _navigationManager.GetUriWithQueryParameters(confirmEmailUri, parameters);
        }
    }
}
