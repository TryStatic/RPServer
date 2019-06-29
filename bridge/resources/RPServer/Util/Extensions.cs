using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using GTANetworkAPI;
using RPServer.Models;

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

            return new EmailAddressAttribute().IsValid(emailString);
        }

        #endregion

        #region MySQLExtensions
        public static int GetInt32Extended(this DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return !reader.IsDBNull(ordinal) ? reader.GetInt32(ordinal) : -1;
        }
        public static string GetStringExtended(this DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return !reader.IsDBNull(ordinal) ? reader.GetString(ordinal) : null;
        }
        public static DateTime GetDateTimeExtended(this DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return !reader.IsDBNull(ordinal) ? reader.GetDateTime(ordinal) : DateTime.MinValue;
        }
        public static bool GetBooleanExtended(this DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.GetBoolean(ordinal);
        }


        #endregion

        #region ClientExtensions
        internal static bool IsLoggedIn(this Client player)
        {
            return player.HasData(Account.DataKey);
        }

        internal static Account GetAccountData(this Client player)
        {
            return player.IsLoggedIn() ? (Account)player.GetData(Account.DataKey) : null;
        }

        internal static bool Login(this Client player, Account account)
        {
            if (player.IsLoggedIn()) return false;
            player.SetData(Account.DataKey, account);
            return true;
        }

        internal static bool Logout(this Client player)
        {
            if (!player.IsLoggedIn()) return false;
            player.ResetData(Account.DataKey);
            return true;
        }

        internal static bool CanRunTask(this Client player)
        {
            return player != null && (bool) player.GetData("CAN_RUN_TASK");
        }

        internal static void SetCanRunTask(this Client player, bool state)
        {
            if(player != null) player.SetData("CAN_RUN_TASK", state);
        }

        #endregion
    }
}