using System;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.InternalAPI.Extensions;
using RPServer.Resource;
using RPServer.Util;

namespace RPServer.Game
{
    internal class Terminate : Script
    {
        [Command(CmdStrings.CMD_Shutdown)]
        public async void CMD_Shutdown(Client client)
        {
            NAPI.Chat.SendChatMessageToAll("[SERVER]: Shutdown inititated, saving data...");
            Initialization.OnServerShutdown();
            Logger.GetInstance().ServerInfo("[SHUTDOWN]: Started saving World Data.");
            await WorldHandler.OnServerShutdown();
            SaveOnlinePlayers();
            Logger.GetInstance().ServerInfo("[SHUTDOWN]: Saving proccess finished, shutting down... ");
            NAPI.Chat.SendChatMessageToAll("[SERVER]: Saving proccess finished, shutting down...");
            Environment.Exit(1);
        }

        private static void SaveOnlinePlayers()
        {
            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                Logger.GetInstance().ServerInfo("[SHUTDOWN]: Started saving Accounts.");
                p.GetAccount()?.UpdateAsync();
                Logger.GetInstance().ServerInfo("[SHUTDOWN]: Started saving Characters.");
                p.GetActiveChar()?.SaveAllData();
            }
        }
    }
}
