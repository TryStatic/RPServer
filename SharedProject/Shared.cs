namespace Shared
{
    public static class ClientToServer
    {
        // Authentication Events
        public const string SubmitRegisterAccount = "SubmitRegisterAccount";
        public const string SubmitLoginAccount = "SubmitLoginAccount";
        public const string SubmitEmailToken = "SubmitEmailToken";
        public const string SubmitGoogleAuthCode = "SubmitGoogleAuthCode";
        public const string SubmitFirstEmailToken = "SubmitFirstEmailToken";
        public const string SubmitNewVerificationEmail = "SubmitNewVerificationEmail";
        public const string SubmitResendEmail = "SubmitResendEmail";
        public const string SubmitBackToLogin = "SubmitBackToLogin";
        public const string SubmitEnableGoogleAuthCode = "SubmitEnableGoogleAuthCode";

        // Character
        public const string SubmitCharacterSelection = "SubmitCharacterSelection";
        public const string SubmitSpawnCharacter = "SubmitSpawnCharacter";

        // Other
        public const string SubmitPlayerCommand = "SubmitPlayerCommand";
    }

    public static class ServerToClient
    {
        // Authentication Events
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

        // Character
        public const string InitCharSelection = "InitCharSelection";
        public const string RenderCharacterList = "RenderCharacterList";
        public const string EndCharSelection = "EndCharSelection";
    }


    public static class SharedDataKey
    {
        public const string AccountLoggedIn = "ACC_LOGGED_IN";
        public const string ActiveCharID = "ACC_LOGGED_IN";
    }

    public struct CharDisplay
    {
        public int CharID;
        public string CharName;

        public CharDisplay(int charID, string charName)
        {
            CharID = charID;
            CharName = charName;
        }
    }
}
