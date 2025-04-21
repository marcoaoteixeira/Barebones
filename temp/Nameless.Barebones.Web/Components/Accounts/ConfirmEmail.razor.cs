using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.FluentUI.AspNetCore.Components;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class ConfirmEmail {
    private readonly UserManager<User> _userManager;
    private readonly IRedirectManager _redirectManager;
    private readonly IToastService _toastService;
    private readonly IStringLocalizer<ConfirmEmail> _localizer;

    public ConfirmEmail(UserManager<User> userManager,
                        IRedirectManager redirectManager,
                        IToastService toastService,
                        IStringLocalizer<ConfirmEmail> localizer) {
        _userManager = Prevent.Argument.Null(userManager);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _toastService = Prevent.Argument.Null(toastService);
        _localizer = Prevent.Argument.Null(localizer);
    }

    private string? StatusMessage { get; set; }

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromQuery]
    private string? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    protected override async Task OnInitializedAsync() {
        //Prevent.Argument.Null(HttpContext);

        if (UserId is null || Code is null) {
            _toastService.ShowInfo("Hohoho!", timeout: 10000);

            return;
            //_redirectManager.Redirect();
        }

        var user = await _userManager.FindByIdAsync(UserId);
        if (user is null) {
            //HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            StatusMessage = _localizer["Error loading user information."];
        } else {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded
                ? _localizer["Thank you for confirming your email."]
                : _localizer["Error confirming your email."];
        }
    }
}
