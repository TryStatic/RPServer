namespace Shared.Events.ClientToServer
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
}