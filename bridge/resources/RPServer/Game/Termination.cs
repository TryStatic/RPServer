using System;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.InternalAPI.Extensions;
using RPServer.Resource;
using RPServer.Util;

namespace RPServer.Game
{
    internal class Termination : Script
    {
        [Command(CommandHandler.ShutdownText)]
        public async void CMD_Shutdown(Client client)
        {
            if(!CommandHandler.Shutdown.IsAuthorized(client)) return;

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
                p.GetActiveChar()?.SaveAllData(p);
            }
        }
    }
}