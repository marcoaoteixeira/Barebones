namespace Nameless.Barebones.Web;

internal static class Constants {
    internal const string ApplicationName = "Barebones";

    internal const string SignInCallbackAction = "SignInCallback";
    internal const string LinkSignInCallbackAction = "LinkSignInCallback";
    internal const string StatusCookieName = "STATUS_MESSAGE";

    internal static class Urls {
        internal const string Auth = "/auth";
        internal const string Error = "/error";
        internal const string Home = "/";

        internal const string AccessDenied = "/access-denied";
        internal const string SignIn = "/signin";
        internal const string SignOut = "/signout";
        internal const string SignUp = "/signup";
        internal const string PerformExternalSignIn = "/perform-external-signin";
        internal const string LinkExternalSignInProvider = "/link-external-signin-provider";

        internal static class Accounts {
            internal const string ConfirmEmail = "/accounts/confirm-email";
            internal const string ConfirmEmailChange = "/accounts/confirm-email-change";
            internal const string ExternalLogin = "/accounts/external-login";
            internal const string ForgotPassword = "/accounts/forgot-password";
            internal const string ForgotPasswordConfirmation = "/accounts/forgot-password-confirmation";
            internal const string InvalidPasswordReset = "/accounts/invalid-password-reset";
            internal const string InvalidUser = "/accounts/invalid-user";
            internal const string Lockout = "/accounts/lockout";
            internal const string SignInWithRecoveryCode = "/accounts/signin-recovery-code";
            internal const string SignInWithTwoFactor = "/accounts/signin-two-factor";
            internal const string ResendEmailConfirmation = "/accounts/resend-email-confirmation";
            internal const string ResetPassword = "/accounts/reset-password";
            internal const string ResetPasswordConfirmation = "/accounts/reset-password-confirmation";
            internal const string SignUpConfirmation = "/accounts/signup-confirmation";

            internal static class Management {
                internal const string ChangePassword = "/accounts/manage/change-password";
                internal const string DeletePersonalData = "/accounts/manage/delete-personal-data";
                internal const string DisableTwoFactorAuthentication = "/accounts/manage/disable-two-factor-auth";
                internal const string Email = "/accounts/manage/email";
                internal const string EnableAuthenticator = "/accounts/manage/enable-authenticator";
                internal const string ExternalLogins = "/accounts/manage/external-logins";
                internal const string GenerateRecoveryCodes = "/accounts/manage/generate-recovery-codes";
                internal const string Index = "/accounts/manage";
                internal const string PersonalData = "/accounts/manage/personal-data";
                internal const string ResetAuthenticator = "/accounts/manage/reset-authenticator";
                internal const string SetPassword = "/accounts/manage/set-password";
                internal const string TwoFactorAuthentication = "/accounts/manage/two-factor-authentication";
            }
        }
    }
}
