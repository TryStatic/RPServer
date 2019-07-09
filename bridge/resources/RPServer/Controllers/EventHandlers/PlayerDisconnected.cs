using GTANetworkAPI;
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
                var acc = client.GetAccountData();
                TaskManager.Run(client, async () => await acc.UpdateAsync(), force: true);
                str = $"Registered (user: {acc.Username}";
                client.Logout();
            }

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
