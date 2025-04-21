using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Web.Endpoints.Groups;

namespace Nameless.Barebones.Web.Endpoints;

public sealed record LinkExternalSignInProviderInput {
    public string Provider { get; init; } = string.Empty;
}

public class LinkExternalSignInProvider : Endpoint<LinkExternalSignInProviderInput, ChallengeHttpResult> {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SignInManager<User> _signInManager;

    public LinkExternalSignInProvider(IHttpContextAccessor httpContextAccessor, SignInManager<User> signInManager) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _httpContextAccessor = Prevent.Argument.Null(httpContextAccessor);
    }

    public override void Configure() {
        Post(Constants.Urls.LinkExternalSignInProvider);
        Group<ManagementGroup>();
    }

    public override async Task HandleAsync(LinkExternalSignInProviderInput request, CancellationToken cancellationToken) {
        var httpContext = Prevent.Argument.Null(_httpContextAccessor.HttpContext);

        // Clear the existing external cookie to ensure a clean login process
        await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var redirectUrl = UriHelper.BuildRelative(httpContext.Request.PathBase,
                                                  "/Account/Manage/ExternalLogins",
                                                  QueryString.Create("Action", Constants.LinkSignInCallbackAction));
        var provider = Utils.RemoveRedundantProvider(request.Provider);
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider,
                                                                                  redirectUrl,
                                                                                  _signInManager.UserManager.GetUserId(httpContext.User));
        var result = TypedResults.Challenge(properties, [provider]);

        await SendResultAsync(result);
    }
}
