namespace Shared.Events.ServerToClient
{
    public static class Authentication
    {
        public const string SetLoginScreen = "SetLoginScreen";
        public const string DisplayError = "DisplayError";
        public const string RegistrationSuccess = "RegistrationSuccess";
        public const string Show2FAbyEmailAddress = "Show2FAbyEmailAddress";
        public const string Show2FAbyGoogleAuth = "Show2FAbyGoogleAuth";
        public const string ShowInitialEmailVerification = "ShowInitialEmailVerification";
        public const string ShowChangeEmailAddress = "ShowChangeEmailAddress";
        public const string ShowLoginPage = "ShowLoginPage";
        public const string ShowQRCode = "ShowQRCode";
        public const string ShowQRCodeEnabled = "ShowQRCodeEnabled";
    }
}
