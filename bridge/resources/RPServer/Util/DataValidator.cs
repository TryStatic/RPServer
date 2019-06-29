using System;
using RPServer.Models;

namespace RPServer.Util
{
    internal static class DataValidator
    {
        public enum ValidationStrings
        {
            Username,
            Password,
            EmailAddress,
            EmailVerificationCode,
            GoogleAuthenticatorCode
        }

        public static bool ValidateString(ValidationStrings strings, string data)
        {
            switch (strings)
            {
                case ValidationStrings.Username: // Username must be at least 4 chars (maybe add settings to tweak these later on)
                    if (string.IsNullOrWhiteSpace(data) || data.Length < 4) return false;
                    break;
                case ValidationStrings.Password: // pass must be at least 4 chars
                    if (string.IsNullOrWhiteSpace(data) || data.Length < 4) return false;
                    break;
                case ValidationStrings.EmailAddress: // Is actually an email address
                    if (string.IsNullOrWhiteSpace(data) || !data.IsValidEmail()) return false;
                    break;
                case ValidationStrings.EmailVerificationCode: // Provided token's length must much whichever the length is on our side
                    if (string.IsNullOrWhiteSpace(data) || data.Length < EmailToken.Length) return false;
                    break;
                case ValidationStrings.GoogleAuthenticatorCode:
                    if (string.IsNullOrWhiteSpace(data) || data.Length < 6 || !IsDigitsOnly(data)) return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strings), strings, null);
            }
            return true;
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}
