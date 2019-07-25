using GTANetworkAPI;
using RPServer.Controllers.Util;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;

namespace RPServer.Controllers.EventHandlers
{
    internal class PlayerDisconnected : Script
    {
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client client, DisconnectionType type, string reason)
        {
            var str = $"Player (name: {client.Name}";
            if (client.IsLoggedIn())
            {
                var accData = client.GetAccount();
                var chData = client.GetActiveChar();

                str = $"Registered (user: {accData.Username}";

                TaskManager.Run(client, async () =>
                {
                    await accData.UpdateAsync();
                    if (chData != null)
                    {
                        await chData.SaveAll();
                    }
                }, force: true);

                client.ResetActiveChar();
                client.Logout();
            }

            // Reset the action queue for that client
            client.ResetActionQueueTimer();

            switch (type)
            {
                case DisconnectionType.Left:
                    Logger.GetInstance().AuthLog($"{str}, social: {client.SocialClubName}, IP: {client.Address}) has left the server."); break;
                case DisconnectionType.Timeout:
                    Logger.GetInstance().AuthLog($"{str}, social: {client.SocialClubName}, IP: {client.Address}) has left the server. (timed out)"); break;
                case DisconnectionType.Kicked:
                    Logger.GetInstance().AuthLog($"{str}, social: {client.SocialClubName}, IP: {client.Address}) has been kicked off the server. Reason: {reason}"); break;
                default: // bug in rageMP API can cause this to run
                    Logger.GetInstance().AuthLog($"{str}, social: {client.SocialClubName}, IP: {client.Address}) has left the server. (default case)"); break;

            }
        }
    }
}
