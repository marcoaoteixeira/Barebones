using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class ExternalSignInProviders {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextUserAccessor _userAccessor;
    private readonly IUserStore<User> _userStore;
    private readonly IRedirectManager _redirectManager;

    private User user = default!;
    private IList<UserLoginInfo>? currentLogins;
    private IList<AuthenticationScheme>? otherLogins;
    private bool showRemoveButton;

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    private string? LoginProvider { get; set; }

    [SupplyParameterFromForm]
    private string? ProviderKey { get; set; }

    [SupplyParameterFromQuery]
    private string? Action { get; set; }

    public ExternalSignInProviders(UserManager<User> userManager,
                                   SignInManager<User> signInManager,
                                   IHttpContextUserAccessor userAccessor,
                                   IUserStore<User> userStore,
                                   IRedirectManager redirectManager) {
        _userManager = Prevent.Argument.Null(userManager);
        _signInManager = Prevent.Argument.Null(signInManager);
        _userAccessor = Prevent.Argument.Null(userAccessor);
        _userStore = Prevent.Argument.Null(userStore);
        _redirectManager = Prevent.Argument.Null(redirectManager);
    }

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        user = await _userAccessor.GetCurrentUserAsync(HttpContext);
        currentLogins = await _userManager.GetLoginsAsync(user);
        otherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider))
            .ToList();

        string? passwordHash = null;
        if (_userStore is IUserPasswordStore<User> userPasswordStore) {
            passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
        }

        showRemoveButton = passwordHash is not null || currentLogins.Count > 1;

        if (HttpMethods.IsGet(HttpContext.Request.Method) && Action == Constants.LinkSignInCallbackAction) {
            await OnGetLinkLoginCallbackAsync();
        }
    }

    private async Task OnSubmitAsync() {
        Prevent.Argument.Null(HttpContext);

        var result = await _userManager.RemoveLoginAsync(user, LoginProvider!, ProviderKey!);
        if (!result.Succeeded) {
            _redirectManager.Redirect("Error: The external login was not removed.", HttpContext);
        }

        await _signInManager.RefreshSignInAsync(user);
        _redirectManager.Redirect("The external login was removed.", HttpContext);
    }

    private async Task OnGetLinkLoginCallbackAsync() {
        Prevent.Argument.Null(HttpContext);

        var userId = await _userManager.GetUserIdAsync(user);
        var info = await _signInManager.GetExternalLoginInfoAsync(userId);
        if (info is null) {
            _redirectManager.Redirect("Error: Could not load external login info.", HttpContext);
        }

        var result = await _userManager.AddLoginAsync(user, info);
        if (!result.Succeeded) {
            _redirectManager.Redirect("Error: The external login was not added. External logins can only be associated with one account.", HttpContext);
        }

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        _redirectManager.Redirect("The external login was added.", HttpContext);
    }
}
