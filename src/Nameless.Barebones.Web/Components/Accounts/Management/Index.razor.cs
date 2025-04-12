using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts.Management;

public partial class Index {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextUserAccessor _userAccessor;
    private readonly IRedirectManager _redirectManager;

    private User user = default!;
    private string? username;
    private string? phoneNumber;

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    public Index(UserManager<User> userManager,
                 SignInManager<User> signInManager,
                 IHttpContextUserAccessor userAccessor,
                 IRedirectManager redirectManager) {
        _userManager = Prevent.Argument.Null(userManager);
        _signInManager = Prevent.Argument.Null(signInManager);
        _userAccessor = Prevent.Argument.Null(userAccessor);
        _redirectManager = Prevent.Argument.Null(redirectManager);
    }

    protected override async Task OnInitializedAsync() {
        Prevent.Argument.Null(HttpContext);

        user = await _userAccessor.GetCurrentUserAsync(HttpContext);
        username = await _userManager.GetUserNameAsync(user);
        phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        Input.PhoneNumber ??= phoneNumber;
    }

    private async Task OnValidSubmitAsync() {
        Prevent.Argument.Null(HttpContext);

        if (Input.PhoneNumber != phoneNumber) {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded) {
                _redirectManager.Redirect("Error: Failed to set phone number.", HttpContext);
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        _redirectManager.Redirect("Your profile has been updated", HttpContext);
    }

    private sealed class InputModel {
        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }
    }
}
