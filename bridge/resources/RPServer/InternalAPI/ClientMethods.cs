using System;
using GTANetworkAPI;

namespace RPServer.InternalAPI
{
    internal static class ClientMethods
    {
        internal static Client FindClient(string identifier)
        {
            if (!IsNumeric(identifier, out var someID))
            {
                var pList = NAPI.Pools.GetAllPlayers();
                var matches = pList.FindAll(i => i.Name.StartsWith(identifier, StringComparison.OrdinalIgnoreCase));
                return matches.Count == 1 ? matches[0] : null;
            }

            if (IsPlayerID(someID)) return FindClientByPlayerID(someID);
            if (IsAliasID(someID)) return FindClientByAliasID(someID);

            return null;
        }

        /// <summary>
        /// Finds Client by an ID
        /// </summary>
        /// <param name="id">Either PlayerID or AliasID</param>
        /// <returns></returns>
        internal static Client FindClientByID(int id)
        {
            if (IsPlayerID(id)) return FindClientByPlayerID(id);
            if (IsAliasID(id)) return FindClientByAliasID(id);
            return null;
        }

        internal static Client FindClientByPlayerID(int id) => IsPlayerID(id) ? NAPI.Pools.GetAllPlayers().Find(p => p.Value == id) : null;

        internal static Client FindClientByAliasID(int id)
        {
            if (IsAliasID(id)) return null;

            return null; // TODO: IMPLEMENT ME
        }

        internal static bool IsPlayerID(int id) => id >= 0 && id < NAPI.Server.GetMaxPlayers(); // 0 <= id < max_players

        internal static bool IsAliasID(int id) => id >= NAPI.Server.GetMaxPlayers(); // id >= max_players

        private static bool IsNumeric(string identifier, out int outID)
        {
            var isNumeric = int.TryParse(identifier, out var someID);
            outID = someID;
            return isNumeric;
        }
    }
}
