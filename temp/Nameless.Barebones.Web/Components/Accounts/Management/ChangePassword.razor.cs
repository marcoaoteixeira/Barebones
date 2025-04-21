using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class ChangePassword {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextUserAccessor _httpContextUserAccessor;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<ChangePassword> _logger;

    private string? message;
    private User user = default!;
    private bool hasPassword;

    public ChangePassword(UserManager<User> userManager,
                          SignInManager<User> signInManager,
                          IHttpContextUserAccessor httpContextUserAccessor,
                          IRedirectManager redirectManager,
                          ILogger<ChangePassword> logger) {
        _userManager = Prevent.Argument.Null(userManager);
        _signInManager = Prevent.Argument.Null(signInManager);
        _httpContextUserAccessor = Prevent.Argument.Null(httpContextUserAccessor);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _logger = Prevent.Argument.Null(logger);
    }

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        user = await _httpContextUserAccessor.GetCurrentUserAsync(HttpContext);
        hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword) {
            _redirectManager.Redirect("Account/Manage/SetPassword");
        }
    }

    private async Task OnValidSubmitAsync() {
        Prevent.Argument.Null(HttpContext);

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded) {
            message = $"Error: {string.Join(",", changePasswordResult.Errors.Select(error => error.Description))}";
            return;
        }

        await _signInManager.RefreshSignInAsync(user);
        _logger.LogInformation("User changed their password successfully.");

        _redirectManager.Redirect("Your password has been changed", HttpContext);
    }

    private sealed class InputModel {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
