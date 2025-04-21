using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class SetPassword {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextUserAccessor _userAccessor;
    private readonly IRedirectManager _redirectManager;

    private string? message;
    private User user = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    public SetPassword(UserManager<User> userManager,
                       SignInManager<User> signInManager,
                       IHttpContextUserAccessor userAccessor,
                       IRedirectManager redirectManager) {
        _userManager = Prevent.Argument.Null(userManager);
        _signInManager = Prevent.Argument.Null(signInManager);
        _userAccessor = Prevent.Argument.Null(userAccessor);
        _redirectManager = Prevent.Argument.Null(redirectManager);
    }

    protected override async Task OnInitializedAsync() {
        user = await _userAccessor.GetCurrentUserAsync(HttpContext);

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (hasPassword) {
            _redirectManager.Redirect("Account/Manage/ChangePassword");
        }
    }

    private async Task OnValidSubmitAsync() {
        var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword!);
        if (!addPasswordResult.Succeeded) {
            message = $"Error: {string.Join(",", addPasswordResult.Errors.Select(error => error.Description))}";
            return;
        }

        await _signInManager.RefreshSignInAsync(user);
        _redirectManager.Redirect("Your password has been set.", HttpContext);
    }

    private sealed class InputModel {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
