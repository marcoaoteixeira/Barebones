using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class TwoFactorAuthentication {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextUserAccessor _userAccessor;
    private readonly IRedirectManager _redirectManager;

    private bool canTrack;
    private bool hasAuthenticator;
    private int recoveryCodesLeft;
    private bool is2faEnabled;
    private bool isMachineRemembered;

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    public TwoFactorAuthentication(UserManager<User> userManager,
                                   SignInManager<User> signInManager,
                                   IHttpContextUserAccessor userAccessor,
                                   IRedirectManager redirectManager) {
        _userManager = Prevent.Argument.Null(userManager);
        _signInManager = Prevent.Argument.Null(signInManager);
        _userAccessor = Prevent.Argument.Null(userAccessor);
        _redirectManager = Prevent.Argument.Null(redirectManager);
    }

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        var user = await _userAccessor.GetCurrentUserAsync(HttpContext);
        canTrack = HttpContext.Features.Get<ITrackingConsentFeature>()?.CanTrack ?? true;
        hasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) is not null;
        is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        isMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);
        recoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);
    }

    private async Task OnSubmitForgetBrowserAsync() {
        Prevent.Argument.Null(HttpContext);

        await _signInManager.ForgetTwoFactorClientAsync();

        _redirectManager.Redirect("The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.",
                                 HttpContext);
    }
}
