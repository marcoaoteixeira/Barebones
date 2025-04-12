using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nameless.WebApplication.Application.Internals;
using Nameless.WebApplication.Domains.Entities.Identity;

namespace Nameless.WebApplication.Application;
public sealed class HttpContextUserAccessor : IUserAccessor {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRedirectManager _redirectManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<HttpContextUserAccessor> _logger;

    public HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor,
                                   IRedirectManager redirectManager,
                                   UserManager<User> userManager,
                                   ILogger<HttpContextUserAccessor> logger) {
        _httpContextAccessor = Prevent.Argument.Null(httpContextAccessor);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _userManager = Prevent.Argument.Null(userManager);
        _logger = Prevent.Argument.Null(logger);
    }

    public async Task<User> GetCurrentUserAsync() {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null) {
            _logger.HttpContextUnavailable();

            _redirectManager.Redirect(uri: Constants.Urls.Pages.Identity.Accounts.InvalidUser);
        }

        var currentUser = await _userManager.GetUserAsync(httpContext.User);

        if (currentUser is null) {
            _logger.CurrentUserUnavailable();

            _redirectManager.Redirect(uri: Constants.Urls.Pages.Identity.Accounts.InvalidUser,
                                      statusMessage: $"Error: Unable to load user with ID '{_userManager.GetUserId(httpContext.User)}'.",
                                      httpContext: httpContext);
        }

        return currentUser;
    }
}
