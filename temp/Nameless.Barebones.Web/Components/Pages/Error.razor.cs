using System.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace Nameless.Barebones.Web.Components.Pages;

public partial class Error {
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }
    private string RequestId { get; set; } = string.Empty;
    private bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);

    protected override void OnInitialized()
        => RequestId = Activity.Current?.Id
                       ?? HttpContext?.TraceIdentifier
                       ?? string.Empty;
}
