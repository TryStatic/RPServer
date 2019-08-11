namespace RPServer.Resource
{
    public static class AccountStrings
    {
        public const string ErrorAccountAlreadyLoggedIn = "This account is already logged into the server.";

        public const string ErrorChangeVerificationEmailDuplicate =
            "The new email address must be different than the old one.";

        public const string ErrorEmailAlreadyVerified = "Your email address is already verified.";
        public const string ErrorEmailInvalid = "This is not a valid email address.";
        public const string ErrorEmailTaken = "That email address is already taken.";
        public const string ErrorEmailTokenAddressTaken = "That email address is already set to be verified.";

        public const string ErrorInvalidCredentials =
            "You have provided invalid an invalid username/password combination.";

        public const string ErrorInvalidVerificationCode = "You have provided an invalid verification token.";
        public const string ErrorPasswordInvalid = "This is not a valid password.";
        public const string ErrorPlayerAlreadyLoggedIn = "You are already logged in.";
        public const string ErrorPlayerNotLoggedIn = "You are not logged in.";
        public const string ErrorUsernameInvalid = "This is not a valid username.";
        public const string ErrorUsernameNotExist = "We couldn't a record related to that username.";
        public const string ErrorUsernameTaken = "That username is already taken.";
        public const string InfoWelcome = "Hello welcome to our server.";

        public const string SuccessChangeVerificationEmailAddress =
            "You have changed your verification email address. Check your inbox.";

        public const string SuccessEmailVerification =
            "Congratulations, you have successfully verified your email address. Welcome to our server.";

        public const string SuccessLogin = "You have successfully logged in, welcome back.";
        public const string SuccessRegistration = "Congratulations, you have successfully registered a new account.";

        public const string SuccessResendVerificationEmail =
            "We have re-sent an email containing your verification token to your inbox.";

        public const string VerifyTwoFactorByGA = "Please verify using the Google Authenticator App.";

        public const string TwoFactorByEmailIsNotEnabled =
            "Two Factor Authentication by Email is not enabled for this account.";
    }
}