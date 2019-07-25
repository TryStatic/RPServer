using System;
using System.Linq;
using System.Net.Mail;

namespace RPServer.Util
{
    public static class Extensions
    {
        #region StringExtensions
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return char.IsUpper(input.First()) ? input : input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
        public static bool IsValidEmail(this string emailString)
        {// TODO: Implement better way for validating emails
            if (string.IsNullOrWhiteSpace(emailString))
                return false;

            try
            {
                var m = new MailAddress(emailString);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        #endregion
    }
}