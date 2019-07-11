using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Newtonsoft.Json;
using RPServer.Models;
using RPServer.Resource;
using RPServer.Util;
using Shared;

namespace RPServer.Controllers
{
    internal class CharacterHandler : Script
    {
        [Command("removecam")]
        public void cmd_removecam(Client client)
        { // Temporary (for testing)
            client.TriggerEvent(ServerToClient.EndCharSelection);
        }
        [Command("changechar")]
        public void cmd_ChangeChar(Client client)
        { // Temporary (?)
            if (!client.HasActiveChar())
            {
                client.SendChatMessage("You are not spawned yet.");
                return;
            }

            var ch = client.GetActiveChar();
            ch?.UpdateAsync();
            client.ResetActiveChar();

            InitCharacterSelection(client);
        }
        [Command("selectchar")]
        public void cmd_selectchar(Client client, int id)
        { // Temporary
            client.TriggerEvent("selectchar", id);
        }
        [Command("play")]
        public void cmd_selectchar(Client client)
        { // Temporary
            client.TriggerEvent("playchar");
        }
        [Command("ranomizeappearance")]
        public void cmd_randomapp(Client client, int id)
        { // Temporary
            // Todo: Invoke Client Event to randomize appearance
        }

        public CharacterHandler() => AuthenticationHandler.PlayerSuccessfulLogin += PlayerSuccessfulLogin;

        [RemoteEvent(ClientToServer.ApplyCharacterEditAnimation)]
        public void ClientEvent_ApplyCharacterEditAnimation(Client client) => client.PlayAnimation("missbigscore2aleadinout@ig_7_p2@bankman@", "leadout_waiting_loop", 1);

        [RemoteEvent(ClientToServer.SubmitCharacterSelection)]
        public void ClientEvent_SubmitCharacterSelection(Client client, int selectedCharId)
        {
            if(!client.IsLoggedIn()) return;
            if(selectedCharId < 0) return;

            TaskManager.Run(client, async () =>
            {
                var fetchedChar = await Character.ReadAsync(selectedCharId);
                var accData = client.GetAccount();

                if (accData.ID != fetchedChar.CharOwnerID)
                {
                    client.SendChatMessage("That is not your character. Ban/Kick?");
                    return;
                }

                var app = await fetchedChar.GetAppearance();
                if(app != null) app.Apply(client);
                client.Transparency = 255;
            });
        }

        [RemoteEvent(ClientToServer.SubmitSpawnCharacter)]
        public void ClientEvent_SubmitSpawnCharacter(Client client, int selectedCharId)
        {
            if(selectedCharId < 0) return;

            TaskManager.Run(client, async () =>
            {
                var chData = await Character.ReadAsync(selectedCharId);
                var accData = client.GetAccount();

                if (chData.CharOwnerID != accData.ID)
                {
                    client.SendChatMessage("That is not your character. Ban/Kick?");
                    return;
                }

                accData.LastSpawnedCharId = selectedCharId;
                client.Dimension = 0;
                client.Transparency = 255;
                client.SendChatMessage("TODO: Teleport to last known position here");
                client.Position = new Vector3(-173.1077, 434.9248, 111.0801); // dummy
                client.SetActiveChar(chData);
                client.TriggerEvent(ServerToClient.EndCharSelection);
            });
        }


        private static void PlayerSuccessfulLogin(object source, EventArgs e)
        {
            var client = source as Client;
            if (client == null) return;
            if (!client.IsLoggedIn()) return;

            InitCharacterSelection(client);
        }
        private static void InitCharacterSelection(Client client)
        {
            client.TriggerEvent(ServerToClient.InitCharSelection);
            client.ResetActiveChar();
            client.Transparency = 0;
            client.Dimension = (uint)client.Value + 1500;

            TaskManager.Run(client, async () =>
            {
                var acc = client.GetAccount();
                var charList = await Character.FetchAllAsync(acc);
                var charDisplayList = new List<CharDisplay>();
                foreach (var c in charList)
                {
                    charDisplayList.Add(new CharDisplay(c.ID, c.CharacterName));
                }
                client.TriggerEvent(ServerToClient.RenderCharacterList, JsonConvert.SerializeObject(charDisplayList), acc.LastSpawnedCharId);
            });
        }
    }
}
