using GTANetworkAPI;
using RPServer.Game;
using RPServer.InternalAPI.Extensions;
using RPServer.Resource;
using RPServer.Util;
using Shared.Data;

namespace RPServer.Controllers.EventHandlers
{
    internal class PlayerConnected : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Client client)
        {
            client.Logout();
            client.ResetActiveChar();

            InitializeSharedData(client);

            client.SendChatMessage(AccountStrings.InfoWelcome);
            client.SendChatMessage("To toggle cursor press F2");
            AccountManager.SetLoginState(client, true);
            Logger.GetInstance().AuthLog($"Player (social: {client.SocialClubName}, IP: {client.Address}) has connected to the server.");
        }

        private void InitializeSharedData(Client client)
        {
            // Init all static HUD shared data here
            client.SetSharedData(Keys.ServerVersion, $"{Globals.SERVER_NAME}-{Globals.VERSION}");
            client.SetSharedData(Keys.AccountLoggedIn, false);
            client.SetSharedData(Keys.ActiveCharID, -1);
        }
    }
}