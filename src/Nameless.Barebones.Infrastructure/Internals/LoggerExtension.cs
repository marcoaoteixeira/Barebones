using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nameless.Barebones.Infrastructure.Identity;

namespace Nameless.Barebones.Infrastructure.Internals;

internal static class LoggerExtension {
    private static readonly Action<ILogger<HttpContextUserAccessor>, Exception?>
        HttpContextUnavailableDelegate = LoggerMessage.Define(logLevel: LogLevel.Warning,
                                                              eventId: default,
                                                              formatString: $"{nameof(IHttpContextAccessor)}.{nameof(IHttpContextAccessor.HttpContext)} unavailable.",
                                                              options: null);

    internal static void HttpContextUnavailable(this ILogger<HttpContextUserAccessor> self)
        => HttpContextUnavailableDelegate(self, null /* exception */);

    private static readonly Action<ILogger<HttpContextUserAccessor>, Exception?>
        CurrentUserUnavailableDelegate = LoggerMessage.Define(logLevel: LogLevel.Warning,
                                                              eventId: default,
                                                              formatString: $"Could not retrieve user using {nameof(HttpContext)}.{nameof(HttpContext.User)}.",
                                                              options: null);

    internal static void CurrentUserUnavailable(this ILogger<HttpContextUserAccessor> self)
        => CurrentUserUnavailableDelegate(self, null /* exception */);
}
