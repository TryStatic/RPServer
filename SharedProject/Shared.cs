namespace Shared
{
    public static class Events
    {
        public static class ClientToServer
        {
            public static class Authentication
            {
                public const string SubmitRegisterAccount = "SubmitRegisterAccount";
                public const string SubmitLoginAccount = "SubmitLoginAccount";
                public const string SubmitEmailToken = "SubmitEmailToken";
                public const string SubmitGoogleAuthCode = "SubmitGoogleAuthCode";
                public const string SubmitFirstEmailToken = "SubmitFirstEmailToken";
                public const string SubmitNewVerificationEmail = "SubmitNewVerificationEmail";
                public const string SubmitResendEmail = "SubmitResendEmail";
                public const string SubmitBackToLogin = "SubmitBackToLogin";
                public const string SubmitEnableGoogleAuthCode = "SubmitEnableGoogleAuthCode";
            }

            public static class Character
            {
                public const string SubmitCharacterSelection = "SubmitCharacterSelection";
                public const string SubmitSpawnCharacter = "SubmitSpawnCharacter";
                public const string ApplyCharacterEditAnimation = "ApplyCharacterEditAnimation";
                public const string SubmitInitialCharData = "SubmitInitialCharData";
                public const string SubmitNewCharacter = "SubmitNewCharacter";
                public const string TriggerCharSelection = "TriggerCharSelection";
            }

            public static class Command
            {
                public const string SubmitPlayerCommand = "SubmitPlayerCommand";

            }
        }

        public static class ServerToClient
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

            public static class Character
            {
                public const string InitCharSelector = "InitCharSelector";
                public const string RenderCharacterList = "RenderCharacterList";
                public const string EndCharSelector = "EndCharSelector";
                public const string DisplayCharError = "DisplayCharError";
                public const string StartCustomization = "StartCustomization";
                public const string ResetCharCreation = "ResetCharCreation";
                public const string SuccessCharCreation = "SuccessCharCreation";
            }

        }
    }

    public static class SharedDataKey
    {
        public const string AccountLoggedIn = "ACC_LOGGED_IN";
        public const string ActiveCharID = "ACTIVE_CHAR_ID";
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
