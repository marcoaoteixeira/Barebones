using FastEndpoints;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Web.Endpoints.Groups;

namespace Nameless.Barebones.Web.Endpoints;

public sealed record PerformExternalSignInInput {
    public required string Provider { get; init; }
    public string? ReturnUrl { get; init; }
}

public class PerformExternalSignIn : Endpoint<PerformExternalSignInInput, ChallengeHttpResult> {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SignInManager<User> _signInManager;

    public PerformExternalSignIn(IHttpContextAccessor httpContextAccessor,
                                SignInManager<User> signInManager) {
        _httpContextAccessor = Prevent.Argument.Null(httpContextAccessor);
        _signInManager = Prevent.Argument.Null(signInManager);
    }

    public override void Configure() {
        Post(Constants.Urls.PerformExternalSignIn);
        Group<AccountsGroup>();
    }

    public override Task HandleAsync(PerformExternalSignInInput request, CancellationToken cancellationToken) {
        var query = new KeyValuePair<string, StringValues>[] {
            new("ReturnUrl", request.ReturnUrl),
            new("Action", Constants.SignInCallbackAction)
        };

        var httpContext = Prevent.Argument.Null(_httpContextAccessor.HttpContext);
        var redirectUrl = UriHelper.BuildRelative(pathBase: httpContext.Request.PathBase,
                                                  path: Constants.Urls.Accounts.ExternalLogin,
                                                  query: QueryString.Create(query));

        var provider = Utils.RemoveRedundantProvider(request.Provider);
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        var result = TypedResults.Challenge(properties, [provider]);

        return SendResultAsync(result);
    }
}
