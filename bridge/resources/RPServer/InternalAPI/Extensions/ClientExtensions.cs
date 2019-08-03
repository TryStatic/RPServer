using System;
using System.Collections.Concurrent;
using System.Threading;
using GTANetworkAPI;
using RPServer.Controllers.Util;
using RPServer.Models;
using RPServer.Resource;

namespace RPServer.InternalAPI.Extensions
{
    internal static class ClientExtensions
    {
        // Account
        internal static bool IsLoggedIn(this Client player, bool excludeTwoFactor = false)
        {
            if (excludeTwoFactor) return player.HasData(DataKey.AccountData);
            return player.HasData(DataKey.AccountData) && player.GetAccount().IsTwoFactorAuthenticated();
        }
        internal static AccountModel GetAccount(this Client player)
        {
            return player.IsLoggedIn(true) ? (AccountModel)player.GetData(DataKey.AccountData) : null;
        }
        internal static bool Login(this Client player, AccountModel account)
        {
            if (player.IsLoggedIn(true)) return false;
            player.SetData(DataKey.AccountData, account);
            player.SetSharedData(Shared.Data.Keys.AccountLoggedIn, true);
            return true;
        }
        internal static bool Logout(this Client player)
        {
            if (!player.IsLoggedIn(true)) return false;
            player.ResetData(DataKey.AccountData);
            player.SetSharedData(Shared.Data.Keys.AccountLoggedIn, false);
            return true;
        }

        // Character
        internal static bool HasActiveChar(this Client client)
        {
            if (!client.HasData(DataKey.ActiveCharData)) return false;
            return (CharacterModel)client.GetData(DataKey.ActiveCharData) != null;
        }
        internal static CharacterModel GetActiveChar(this Client client)
        {
            if (!client.HasActiveChar()) return null;
            return (CharacterModel)client.GetData(DataKey.ActiveCharData);
        }
        internal static void SetActiveChar(this Client client, CharacterModel character)
        {
            if (character == null)
            {
                ResetActiveChar(client);
                return;
            }
            client.SetData(DataKey.ActiveCharData, character);
            client.SetSharedData(Shared.Data.Keys.ActiveCharID, character.ID);
        }
        internal static void ResetActiveChar(this Client client)
        {
            client.ResetData(DataKey.ActiveCharData);
            client.SetSharedData(Shared.Data.Keys.ActiveCharID, -1);
        }

    }
}
