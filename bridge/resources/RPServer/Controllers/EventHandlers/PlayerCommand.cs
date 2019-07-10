using GTANetworkAPI;
using RPServer.Util;
using Shared;

namespace RPServer.Controllers.EventHandlers
{
    internal class PlayerCommand : Script
    {
        [RemoteEvent(ClientToServer.SubmitPlayerCommand)]
        public void ClientEvent_OnPlayerCommand(Client client, string cmd)
        { // This CANNOT block commands
            var username = "UNREGISTERED";
            if (client.IsLoggedIn())
            {
                var accData = client.GetAccountData();
                username = accData.Username;
            }
            Logger.GetInstance().CommandLog(username, cmd);
        }
    }
}
