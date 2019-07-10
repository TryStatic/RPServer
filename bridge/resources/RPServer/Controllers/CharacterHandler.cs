using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GTANetworkAPI;
using Newtonsoft.Json;
using RPServer.Models;
using RPServer.Util;
using Shared;

namespace RPServer.Controllers
{
    internal class CharacterHandler : Script
    {
        [Command("removecam")]
        public void cmd_removecam(Client client)
        {
            client.TriggerEvent("debugdestroycam");
        }

        [Command("createchar")]
        public void cmd_createchar(Client client, int id)
        {
            
        }

        [Command("selectchar")]
        public void cmd_selectchar(Client client, int id)
        {
            client.TriggerEvent("selectchar", id);
        }

        [Command("ranomizeappearance")]
        public void cmd_randomapp(Client client, int id)
        {
            
        }

        public CharacterHandler()
        {
            AuthenticationHandler.PlayerSuccessfulLogin += PlayerSuccessfulLogin;
        }

        private void PlayerSuccessfulLogin(object source, EventArgs e)
        {
            var client = source as Client;
            if (client == null) return;
            if(!client.IsLoggedIn()) return;

            client.SendChatMessage("[SERVER]: INIT CHAR SELECTION");
            client.Transparency = 0;
            client.TriggerEvent(ServerToClient.InitCharSelection);

            var accData = client.GetAccountData();
            client.SendChatMessage($"[SERVER]: FETCHING CHARS FOR PLAYER {accData.Username}");
            TaskManager.Run(client, async () =>
            {
                var chars = await Character.FetchAllAsync(accData);
                var charClientList = new List<CharDisplay>();

                foreach (var c in chars)
                {
                    charClientList.Add(new CharDisplay(c.ID, c.CharacterName));
                }
                client.SendChatMessage("[SERVER]: Sending charlist to Client");
                client.TriggerEvent(ServerToClient.RenderCharacterList, JsonConvert.SerializeObject(charClientList));
            });

        }

        [RemoteEvent("ApplyCharSelectionAnimation")]
        public void ClientEvent_ApplyCharSelectionAnimation(Client client) => client.PlayAnimation("missbigscore2aleadinout@ig_7_p2@bankman@", "leadout_waiting_loop", 1);

        [RemoteEvent(ClientToServer.SubmitCharacterSelection)]
        public void ClientEventSubmitCharacterSelection(Client client, int selectedCharId)
        {
            if(!client.IsLoggedIn()) return;
            if(selectedCharId < 0) return;

            TaskManager.Run(client, async () =>
            {
                var fetchedChar = await Character.ReadAsync(selectedCharId);
                //var cus = fetchedChar.CustomSkin;
                var accData = client.GetAccountData();

                if (accData.ID != fetchedChar.CharOwnerID)
                {
                    client.SendChatMessage("That is not your character. Ban/Kick?");
                    return;
                }

                var app = await fetchedChar.GetAppearance();
                if(app == null) throw new Exception($"Character {fetchedChar.CharacterName} ({fetchedChar.ID}) has no appearance data to fetch.");

                app.Apply(client);
                client.Transparency = 255;
            });
        }
    }
}
