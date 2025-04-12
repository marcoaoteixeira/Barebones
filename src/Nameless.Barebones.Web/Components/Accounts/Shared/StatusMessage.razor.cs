using Microsoft.AspNetCore.Components;

namespace Nameless.Barebones.Web.Components.Accounts.Shared;

public partial class StatusMessage : ComponentBase {
    private string? messageFromCookie;

    [Parameter]
    public string? Message { get; set; }

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    private string? DisplayMessage => Message ?? messageFromCookie;

    protected override Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        messageFromCookie = HttpContext.Request.Cookies[Constants.StatusCookieName];

        if (messageFromCookie is not null) {
            HttpContext.Response.Cookies.Delete(Constants.StatusCookieName);
        }

        return Task.CompletedTask;
    }
}
