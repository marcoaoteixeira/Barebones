using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Web.Components.Accounts.Shared;

public partial class ExternalSignInPicker {
    private readonly SignInManager<User> _signInManager;

    private AuthenticationScheme[] externalLogins = [];

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    public ExternalSignInPicker(SignInManager<User> signInManager) {
        _signInManager = Prevent.Argument.Null(signInManager);
    }

    protected override async Task OnInitializedAsync() {
        externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
    }
}
