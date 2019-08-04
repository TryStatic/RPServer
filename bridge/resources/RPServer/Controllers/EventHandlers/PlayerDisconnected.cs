using GTANetworkAPI;
using RPServer.Controllers.Util;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;

namespace RPServer.Controllers.EventHandlers
{
    internal class PlayerDisconnected : Script
    {
        [ServerEvent(Event.PlayerDisconnected)]
        public async void OnPlayerDisconnected(Client client, DisconnectionType type, string reason)
        {
            var str = client.IsLoggedIn(true) ? $"Registered (user: {client.GetAccount().Username}, " : "Player (";
            switch (type)
            {
                case DisconnectionType.Left:
                    Logger.GetInstance().AuthLog($"{str}social: {client.SocialClubName}, IP: {client.Address}) has left the server."); break;
                case DisconnectionType.Timeout:
                    Logger.GetInstance().AuthLog($"{str}social: {client.SocialClubName}, IP: {client.Address}) has left the server. (timed out)"); break;
                case DisconnectionType.Kicked:
                    Logger.GetInstance().AuthLog($"{str}social: {client.SocialClubName}, IP: {client.Address}) has been kicked off the server. Reason: {reason}"); break;
                default: // bug in rageMP API can cause this to run
                    Logger.GetInstance().AuthLog($"{str}social: {client.SocialClubName}, IP: {client.Address}) has left the server. (default case)"); break;
            }

            if (client.IsLoggedIn())
            {
                await AccountManager.LogoutAccount(client);
            }
        }
    }
}
