using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Internals;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Infrastructure.Identity;

public sealed class HttpContextUserAccessor : IHttpContextUserAccessor {
    private const string InvalidUserUrl = "/accounts/invalid-user";

    private readonly IRedirectManager _redirectManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<HttpContextUserAccessor> _logger;

    public HttpContextUserAccessor(IRedirectManager redirectManager,
                                   UserManager<User> userManager,
                                   ILogger<HttpContextUserAccessor> logger) {
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _userManager = Prevent.Argument.Null(userManager);
        _logger = Prevent.Argument.Null(logger);
    }

    public async Task<User> GetCurrentUserAsync(HttpContext httpContext) {
        var currentUser = await _userManager.GetUserAsync(httpContext.User);
        if (currentUser is null) {
            _logger.CurrentUserUnavailable();

            _redirectManager.Redirect(uri: InvalidUserUrl,
                                      statusMessage: "Warning: Unable to load user from HttpContext.",
                                      httpContext: httpContext);
        }

        return currentUser;
    }
}
