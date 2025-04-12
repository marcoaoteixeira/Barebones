using Microsoft.AspNetCore.Components;
using Nameless.Barebones.Infrastructure.Identity;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class PersonalData {
    private readonly IHttpContextUserAccessor _userAccessor;

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    public PersonalData(IHttpContextUserAccessor userAccessor) {
        _userAccessor = Prevent.Argument.Null(userAccessor);
    }
    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        _ = await _userAccessor.GetCurrentUserAsync(HttpContext);
    }
}
