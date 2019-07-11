using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
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

        internal static void InitActionQueue(this Client player)
        {
            if (player == null) return;

            player.SetData(DataKey.ActionQueue, new ConcurrentQueue<Action>());
            player.SetData(DataKey.ActionQueueTimer, new Timer(TaskManager.OnHandleDequeue, player, 1, Timeout.Infinite));
        }

        internal static ConcurrentQueue<Action> GetActionQueue(this Client player)
        {
            if (player == null) return null;
            if (!player.HasData(DataKey.ActionQueue)) throw new Exception("Tried to access ActionQueue before initiliaztion.");
            return player.GetData(DataKey.ActionQueue);
        }

        internal static Timer GetActionQueueTimer(this Client player)
        {
            if (player == null) return null;
            if (!player.HasData(DataKey.ActionQueueTimer)) throw new Exception("Tried to access ActionQueueTimer before initiliaztion.");
            return player.GetData(DataKey.ActionQueueTimer);
        }


        #endregion
    }
}