﻿using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Nameless.Barebones.Domains.Entities.Identity;
using Nameless.Barebones.Infrastructure.Navigation;

namespace Nameless.Barebones.Web.Components.Accounts;

public partial class SignUp {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailSender<User> _emailSender;
    private readonly NavigationManager _navigationManager;
    private readonly IRedirectManager _redirectManager;
    private readonly ILogger<SignUp> _logger;

    private IStringLocalizer<SignUp> T { get; set; }

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private IdentityError[] IdentityErrors { get; set; } = [];

    private string Message => IdentityErrors.Length > 0
        ? $"Error: {string.Join(", ", IdentityErrors.Select(error => error.Description))}"
        : string.Empty;

    public SignUp(UserManager<User> userManager,
                  SignInManager<User> signInManager,
                  IEmailSender<User> emailSender,
                  NavigationManager navigationManager,
                  IRedirectManager redirectManager,
                  IStringLocalizer<SignUp> localizer,
                  ILogger<SignUp> logger) {
        _userManager = Prevent.Argument.Null(userManager);
        _signInManager = Prevent.Argument.Null(signInManager);
        _emailSender = Prevent.Argument.Null(emailSender);
        _navigationManager = Prevent.Argument.Null(navigationManager);
        _redirectManager = Prevent.Argument.Null(redirectManager);
        _logger = Prevent.Argument.Null(logger);

        T = Prevent.Argument.Null(localizer);
    }

    public async Task RegisterUser(EditContext editContext) {
        var user = new User();
        
        await _userManager.SetUserNameAsync(user, Input.Email);

        if (_userManager.SupportsUserEmail) {
            await _userManager.SetEmailAsync(user, Input.Email);
        }

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded) {
            IdentityErrors = result.Errors.ToArray();
            return;
        }

        _logger.LogInformation("User created a new account with password.");

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = _navigationManager.GetUriWithQueryParameters(
            _navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });

        await _emailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

        if (_userManager.Options.SignIn.RequireConfirmedAccount) {
            _redirectManager.Redirect(
                "Account/RegisterConfirmation",
                new Dictionary<string, object?> { ["email"] = Input.Email, ["returnUrl"] = ReturnUrl });
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        _redirectManager.Redirect(ReturnUrl);
    }

    private sealed class InputModel {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
