using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace Nameless.Barebones.Web.Components.Layout;

public partial class NavMenu : IDisposable {
    private readonly NavigationManager _navigationManager;
    private readonly IStringLocalizer<NavMenu> _localizer;

    private bool expanded = true;
    private string? currentUrl;

    public NavMenu(NavigationManager navigationManager,
                   IStringLocalizer<NavMenu> localizer) {
        _navigationManager = Prevent.Argument.Null(navigationManager);
        _localizer = Prevent.Argument.Null(localizer);
    }

    protected override void OnInitialized() {
        currentUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);

        _navigationManager.LocationChanged += OnLocationChanged;
    }

    public void Dispose()
        => _navigationManager.LocationChanged -= OnLocationChanged;

    private void OnLocationChanged(object? sender, LocationChangedEventArgs args) {
        currentUrl = _navigationManager.ToBaseRelativePath(args.Location);

        StateHasChanged();
    }
}
