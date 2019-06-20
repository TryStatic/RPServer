﻿using System;
using RPServer.Models;

namespace RPServer.Util
{
    internal static class DataValidator
    {
        public enum ValidationStrings
        {
            Username,
            Password,
            EmailVerificationCode
        }

        public static bool ValidateString(ValidationStrings strings, string data)
        {
            switch (strings)
            {
                case ValidationStrings.Username: // Username must be at least 4 chars (maybe add settings to tweak these later on)
                    if (string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data) || data.Length < 4) return false;
                    break;
                case ValidationStrings.Password: // pass must be at least 4 chars
                    if (string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data) || data.Length < 4) return false;
                    break;
                case ValidationStrings.EmailVerificationCode: // Provided token's length must much whichever the length is on our side
                    if (string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data) || data.Length < EmailToken.Length) return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strings), strings, null);
            }

            return true;
        }
    }
}