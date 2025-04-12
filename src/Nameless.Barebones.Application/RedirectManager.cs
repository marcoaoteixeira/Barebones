using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Nameless.WebApplication.Application;

public sealed class RedirectManager : IRedirectManager {
    public const string StatusMessageCookieName = "STATUS_MESSAGE";

    private static readonly CookieBuilder StatusCookieBuilder = new() {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5),
    };

    private readonly NavigationManager _navigationManager;

    private string CurrentPath => _navigationManager.ToAbsoluteUri(_navigationManager.Uri)
                                                    .GetLeftPart(UriPartial.Path);

    public RedirectManager(NavigationManager navigationManager) {
        _navigationManager = Prevent.Argument.Null(navigationManager);
    }

    [DoesNotReturn]
    public void Redirect() => Redirect(CurrentPath);

    [DoesNotReturn]
    public void Redirect(string uri) {
        ArgumentNullException.ThrowIfNull(uri);

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative)) {
            uri = _navigationManager.ToBaseRelativePath(uri);
        }

        // During static rendering, NavigateTo throws a NavigationException
        // which is handled by the framework as a redirect.
        // So as long as this is called from a statically rendered Identity
        // component, the InvalidOperationException is never thrown.
        _navigationManager.NavigateTo(uri);

        throw new InvalidOperationException($"{nameof(RedirectManager)} can only be used during static rendering.");
    }

    [DoesNotReturn]
    public void Redirect(string uri, IReadOnlyDictionary<string, object?> queryParameters) {
        var uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri)
                                                .GetLeftPart(UriPartial.Path);

        var newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);

        Redirect(newUri);
    }

    [DoesNotReturn]
    public void Redirect(string statusMessage, HttpContext httpContext)
        => Redirect(CurrentPath, statusMessage, httpContext);

    [DoesNotReturn]
    public void Redirect(string uri, string statusMessage, HttpContext httpContext) {
        httpContext.Response.Cookies.Append(key: StatusMessageCookieName,
                                            value: statusMessage,
                                            options: StatusCookieBuilder.Build(httpContext));
        Redirect(uri);
    }
}