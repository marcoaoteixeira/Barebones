using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class ResetPassword {
    private readonly IRedirectManager _redirectManager;
    private readonly UserManager<User> _userManager;

    private IEnumerable<IdentityError>? identityErrors;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    private string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

    public ResetPassword(IRedirectManager redirectManager,
                         UserManager<User> userManager) {
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _userManager = Prevent.Argument.Null(userManager);
    }

    protected override void OnInitialized() {
        if (Code is null) {
            _redirectManager.Redirect("Account/InvalidPasswordReset");
        }

        Input.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
    }

    private async Task OnValidSubmitAsync() {
        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user is null) {
            // Don't reveal that the user does not exist
            _redirectManager.Redirect("Account/ResetPasswordConfirmation");
        }

        var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded) {
            _redirectManager.Redirect("Account/ResetPasswordConfirmation");
        }

        identityErrors = result.Errors;
    }

    private sealed class InputModel {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        public string Code { get; set; } = "";
    }
}
