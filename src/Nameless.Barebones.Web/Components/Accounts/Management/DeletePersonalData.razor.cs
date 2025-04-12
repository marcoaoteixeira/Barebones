using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class DeletePersonalData {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextUserAccessor _userAccessor;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<DeletePersonalData> _logger;

    private string? message;
    private User user = default!;
    private bool requirePassword;

    public DeletePersonalData(UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IHttpContextUserAccessor userAccessor,
                              IRedirectManager redirectManager,
                              ILogger<DeletePersonalData> logger) {
        _userManager = Prevent.Argument.Null(userManager);
        _signInManager = Prevent.Argument.Null(signInManager);
        _userAccessor = Prevent.Argument.Null(userAccessor);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _logger = Prevent.Argument.Null(logger);
    }

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        Input ??= new();
        user = await _userAccessor.GetCurrentUserAsync(HttpContext);
        requirePassword = await _userManager.HasPasswordAsync(user);
    }

    private async Task OnValidSubmitAsync() {
        if (requirePassword && !await _userManager.CheckPasswordAsync(user, Input.Password)) {
            message = "Error: Incorrect password.";
            return;
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded) {
            throw new InvalidOperationException("Unexpected error occurred deleting user.");
        }

        await _signInManager.SignOutAsync();

        var userId = await _userManager.GetUserIdAsync(user);
        _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

        _redirectManager.Redirect();
    }

    private sealed class InputModel {
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
