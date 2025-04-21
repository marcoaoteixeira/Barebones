using Microsoft.AspNetCore.Components;

namespace Nameless.Barebones.Web.Components.Accounts.Shared;

public partial class RedirectToSignIn {
    private readonly NavigationManager _navigationManager;

    public RedirectToSignIn(NavigationManager navigationManager) {
        _navigationManager = Prevent.Argument.Null(navigationManager);
    }

    protected override void OnInitialized() {
        var url = $"{Constants.Urls.SignIn}?returnUrl={Uri.EscapeDataString(_navigationManager.Uri)}";

        _navigationManager.NavigateTo(url, forceLoad: true);
    }
}
