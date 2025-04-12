using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Web.Components.Accounts.Shared;

public partial class ManageNavMenu {
    private readonly SignInManager<User> _signInManager;

    private bool hasExternalLogins;

    public ManageNavMenu(SignInManager<User> signInManager) {
        _signInManager = Prevent.Argument.Null(signInManager);
    }

    protected override async Task OnInitializedAsync() {
        hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
    }
}
