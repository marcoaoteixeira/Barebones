using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Nameless.Barebones.Web.Components.Accounts.Shared;

public partial class ShowRecoveryCodes {
    private readonly IStringLocalizer<ShowRecoveryCodes> _localizer;

    [Parameter]
    public string[] RecoveryCodes { get; set; } = [];

    [Parameter]
    public string? StatusMessage { get; set; }

    public ShowRecoveryCodes(IStringLocalizer<ShowRecoveryCodes> localizer) {
        _localizer = Prevent.Argument.Null(localizer);
    }
}
