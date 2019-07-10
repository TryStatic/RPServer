using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Threading;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.Models;
using RPServer.Resource;

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

        #region SQLExtensions
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
        public static void AddParameterWithValue(this DbCommand command, string parameterName, object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }
        public static DbCommand CreateCommandWithText(this DbConnection connection, string commandText)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            return command;
        }
        #endregion

        #region ClientExtensions
        internal static bool IsLoggedIn(this Client player, bool excludeTwoFactor = false)
        {
            if (excludeTwoFactor) return player.HasData(DataKey.AccountData);
            return player.HasData(DataKey.AccountData) && player.GetAccountData().IsTwoFactorAuthenticated();
        }

        internal static Account GetAccountData(this Client player)
        {
            return player.IsLoggedIn(true) ? (Account)player.GetData(DataKey.AccountData) : null;
        }

        internal static bool Login(this Client player, Account account)
        {
            if (player.IsLoggedIn(true)) return false;
            player.SetData(DataKey.AccountData, account);
            return true;
        }

        internal static bool Logout(this Client player)
        {
            if (!player.IsLoggedIn(true)) return false;
            player.ResetData(DataKey.AccountData);
            return true;
        }

        internal static bool CanRunTask(this Client player)
        {
            if (player == null) return false;
            if(!player.HasData(DataKey.CanRunTask)) player.SetCanRunTask(true);
            return (bool) player.GetData(DataKey.CanRunTask);
        }

        internal static void SetCanRunTask(this Client player, bool state)
        {
            if (player != null) player.SetData(DataKey.CanRunTask, state);
        }

        #endregion
    }
}