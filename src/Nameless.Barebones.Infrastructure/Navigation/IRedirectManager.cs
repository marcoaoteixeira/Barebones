using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Nameless.Barebones.Infrastructure.Navigation;

/// <summary>
/// Defines a service that can redirect the current request
/// to another URI.
/// </summary>
public interface IRedirectManager {
    /// <summary>
    /// Redirects to the current URI.
    /// </summary>
    [DoesNotReturn]
    void Redirect();

    /// <summary>
    /// Redirects to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to redirect the request.</param>
    [DoesNotReturn]
    void Redirect(string? uri);

    /// <summary>
    /// Redirects to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to redirect the request.</param>
    /// <param name="queryParameters">URI query parameters</param>
    [DoesNotReturn]
    void Redirect(string uri, IReadOnlyDictionary<string, object?> queryParameters);

    /// <summary>
    /// Redirects to the current URI.
    /// </summary>
    /// <param name="statusMessage">A status message.</param>
    /// <param name="httpContext">The HTTP context.</param>
    [DoesNotReturn]
    void Redirect(string statusMessage, HttpContext httpContext);

    /// <summary>
    /// Redirects to the specified URI.
    /// </summary>
    /// <param name="uri">The URI to redirect the request.</param>
    /// <param name="statusMessage">A status message.</param>
    /// <param name="httpContext">The HTTP context.</param>
    [DoesNotReturn]
    void Redirect(string uri, string statusMessage, HttpContext httpContext);
}