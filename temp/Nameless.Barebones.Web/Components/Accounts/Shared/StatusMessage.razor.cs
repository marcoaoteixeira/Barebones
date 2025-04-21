using Microsoft.AspNetCore.Components;

namespace Nameless.Barebones.Web.Components.Accounts.Shared;

public partial class StatusMessage : ComponentBase {
    private string? _messageFromCookie;

    [Parameter]
    public string? Message { get; set; }

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    private string? DisplayMessage
        => Message ?? _messageFromCookie;

    private string StatusMessageClass
        => !string.IsNullOrWhiteSpace(DisplayMessage) &&
           DisplayMessage.StartsWith("Error")
            ? "danger"
            : "success";

    protected override Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        _messageFromCookie = HttpContext.Request
                                        .Cookies[Constants.StatusCookieName];

        if (_messageFromCookie is not null) {
            HttpContext.Response
                       .Cookies
                       .Delete(Constants.StatusCookieName);
        }

        return Task.CompletedTask;
    }
}
