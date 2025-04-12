using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Nameless.WebApplication.Application;

public interface IRedirectManager {
    /// <summary>
    /// Redirects to the current page.
    /// </summary>
    [DoesNotReturn]
    void Redirect();

    /// <summary>
    /// Redirects to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to redirect the request.</param>
    [DoesNotReturn]
    void Redirect(string uri);

    [DoesNotReturn]
    void Redirect(string uri, IReadOnlyDictionary<string, object?> queryParameters);

    [DoesNotReturn]
    void Redirect(string statusMessage, HttpContext httpContext);

    [DoesNotReturn]
    void Redirect(string uri, string statusMessage, HttpContext httpContext);
}