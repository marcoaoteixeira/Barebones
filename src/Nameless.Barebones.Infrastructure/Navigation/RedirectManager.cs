using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Nameless.Barebones.Infrastructure.Navigation;

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

    /// <inheritdoc />
    [DoesNotReturn]
    public void Redirect()
        => Redirect(CurrentPath);

    /// <inheritdoc />
        [DoesNotReturn]
    public void Redirect(string? uri) {
        uri ??= string.Empty;

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

    /// <inheritdoc />
    [DoesNotReturn]
    public void Redirect(string uri, IReadOnlyDictionary<string, object?> queryParameters) {
        Prevent.Argument.Null(uri);
        Prevent.Argument.Null(queryParameters);

        var uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri)
                                                .GetLeftPart(UriPartial.Path);

        var newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);

        Redirect(newUri);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method will also set a cookie with the status message on it.
    /// </remarks>
    [DoesNotReturn]
    public void Redirect(string statusMessage, HttpContext httpContext)
        => Redirect(CurrentPath, statusMessage, httpContext);

    /// <inheritdoc />
    /// <remarks>
    /// This method will also set a cookie with the status message on it.
    /// </remarks>
    [DoesNotReturn]
    public void Redirect(string uri, string statusMessage, HttpContext httpContext) {
        Prevent.Argument.Null(uri);
        Prevent.Argument.Null(statusMessage);
        Prevent.Argument.Null(httpContext);

        httpContext.Response.Cookies.Append(key: StatusMessageCookieName,
                                            value: statusMessage,
                                            options: StatusCookieBuilder.Build(httpContext));
        Redirect(uri);
    }
}