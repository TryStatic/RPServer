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

            return null;
        }

        internal static Client FindClientByPlayerID(int id)
        {
            return IsPlayerID(id) ? NAPI.Pools.GetAllPlayers().Find(p => p.Value == id) : null;
        }

        internal static bool IsPlayerID(int id)
        {
            return id >= 0 && id < NAPI.Server.GetMaxPlayers();
        }

        private static bool IsNumeric(string identifier, out int outID)
        {
            var isNumeric = int.TryParse(identifier, out var someID);
            outID = someID;
            return isNumeric;
        }
    }
}