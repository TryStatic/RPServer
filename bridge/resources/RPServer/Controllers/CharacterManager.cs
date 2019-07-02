using System;
using Shared;
using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Controllers
{
    internal class CharacterManager : Script
    {
        [Command("removecam")]
        public void cmd_removecam(Client client)
        {
            client.TriggerEvent("debugdestroycam");
        }

        public CharacterManager()
        {
            AuthenticationManager.PlayerSuccessfulLogin += PlayerSuccessfulLogin;
        }

        private void PlayerSuccessfulLogin(object source, EventArgs e)
        {
            var client = source as Client;
            if (client == null) return;
            if(!client.IsLoggedIn()) return;
            client.TriggerEvent(ServerToClient.InitCharSelection);
        }

        [RemoteEvent("ApplyCharSelectionAnimation")]
        public void ClientEvent_ApplyCharSelectionAnimation(Client client) => client.PlayAnimation("missbigscore2aleadinout@ig_7_p2@bankman@", "leadout_waiting_loop", 1);
    }
}
