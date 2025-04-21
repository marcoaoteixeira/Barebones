using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Web.Components.Accounts.Shared;

public partial class ExternalSignInPicker {
    private readonly SignInManager<User> _signInManager;
    private readonly IStringLocalizer<ExternalSignInPicker> _localizer;

    private AuthenticationScheme[] ExternalSignInProviders { get; set; } = [];

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    public ExternalSignInPicker(SignInManager<User> signInManager,
                                IStringLocalizer<ExternalSignInPicker> localizer) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _localizer = Prevent.Argument.Null(localizer);
    }

    protected override Task OnInitializedAsync() {
        //ExternalSignInProviders = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToArray();

        ExternalSignInProviders = [
            new AuthenticationScheme("Google", "Google", typeof(NullAuthenticationHandler)),
            new AuthenticationScheme("Github", "Github", typeof(NullAuthenticationHandler)),
            new AuthenticationScheme("StackOverflow", "StackOverflow", typeof(NullAuthenticationHandler))
        ];

        return Task.CompletedTask;
    }
}

public class NullAuthenticationHandler : IAuthenticationHandler {
    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        => Task.CompletedTask;

    public Task<AuthenticateResult> AuthenticateAsync()
        => Task.FromResult(AuthenticateResult.Fail("No auth"));

    public Task ChallengeAsync(AuthenticationProperties? properties)
        => Task.CompletedTask;

    public Task ForbidAsync(AuthenticationProperties? properties)
        => Task.CompletedTask;
}