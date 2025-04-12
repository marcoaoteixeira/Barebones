using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Web.Endpoints.Groups;

namespace Nameless.Barebones.Web.Endpoints;

public sealed record SignOutInput {
    public string? ReturnUrl { get; init; }
}

public class SignOut : Endpoint<SignOutInput, ChallengeHttpResult> {
    private readonly SignInManager<User> _signInManager;

    public SignOut(SignInManager<User> signInManager) {
        _signInManager = Prevent.Argument.Null(signInManager);
    }

    public override void Configure() {
        Post(Constants.Urls.SignOut);
        Group<AccountsGroup>();
    }

    public override async Task HandleAsync(SignOutInput request, CancellationToken cancellationToken) {
        await _signInManager.SignOutAsync();

        var result = TypedResults.LocalRedirect($"~/{request.ReturnUrl}");

        await SendResultAsync(result);
    }
}
