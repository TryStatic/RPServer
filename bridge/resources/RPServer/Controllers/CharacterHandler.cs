using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using GTANetworkAPI;
using Newtonsoft.Json;
using RPServer.Controllers.Util;
using RPServer.InternalAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Models;
using RPServer.Resource;
using Shared.Data;
using Events = Shared.Events;
using static RPServer.Controllers.Util.DataValidator;

namespace RPServer.Controllers
{
    public delegate void OnCharacterSpawnDelegate(object source, EventArgs e);

    internal class CharacterHandler : Script
    {
        public static event OnCharacterSpawnDelegate CharacterSpawn;
        [Command(CmdStrings.CMD_ChangeChar)]
        public void CMD_ChangeChar(Client client)
        { // Temporary (?)
            if (!client.HasActiveChar())
            {
                client.SendChatMessage("You are not spawned yet.");
                return;
            }

            var ch = client.GetActiveChar();
            ch?.SaveAll();
            client.ResetActiveChar();

            InitCharacterSelection(client);
        }
        
        [Command(CmdStrings.CMD_Alias)]
        public void CMD_Alias(Client client, string identifier, string aliasText = "")
        {
            if (!client.IsLoggedIn())
            {
                client.SendChatMessage("You are not logged in.");
                return;
            }

            if (!client.HasActiveChar())
            {
                client.SendChatMessage("You are not properly spawned.");
                return;
            }

            var otherClient = ClientMethods.FindClient(identifier);
            if (otherClient == null)
            {
                client.SendChatMessage("Invalid Player Identifier.");
                return;
            }

            if (!otherClient.IsLoggedIn())
            {
                client.SendChatMessage("That player is not logged in.");
                return;
            }

            if (!otherClient.HasActiveChar())
            {
                client.SendChatMessage("That player is not spawned.");
                return;
            }

            // TODO: DISTANCE CHECK

            var chData = client.GetActiveChar();
            var chOtherData = otherClient.GetActiveChar();

            if (aliasText == "")
            {
                var alias = chData.Aliases.First(i => i.CharID == chData.ID && i.AliasedID == chOtherData.ID);
                if (alias == null)
                {
                    client.SendChatMessage("No Alias set for that player.");
                    return;
                }
                chData.Aliases.Remove(alias);
                ClientEvent_RequestAliasInfo(client, otherClient.Value);
                client.SendChatMessage("Alias removed.");
                return;
            }

            var exist = chData.Aliases.First(i => i.CharID == chData.ID && i.AliasedID == chOtherData.ID);
            if (exist != null)
            {
                exist.AliasName = aliasText;
            }
            else
            {
                chData.Aliases.Add(new Alias(chData, chOtherData, aliasText));
            }

            ClientEvent_RequestAliasInfo(client, otherClient.Value);
            client.SendChatMessage($"Alias set.");
        }

        public CharacterHandler()
        {
            AuthenticationHandler.PlayerSuccessfulLogin += OnPlayerLogin;
            CharacterHandler.CharacterSpawn += OnCharacterSpawn;
        }

        [RemoteEvent(Events.ClientToServer.Character.ApplyCharacterEditAnimation)]
        public void ClientEvent_ApplyCharacterEditAnimation(Client client) => client.PlayAnimation("missbigscore2aleadinout@ig_7_p2@bankman@", "leadout_waiting_loop", 1);

        [RemoteEvent(Events.ClientToServer.Character.SubmitCharacterSelection)]
        public void ClientEvent_SubmitCharacterSelection(Client client, int selectedCharId)
        {
            if (!client.IsLoggedIn()) return;
            if (selectedCharId < 0) return;

            TaskManager.Run(client, async () =>
            {
                var fetchedChar = await Character.ReadAsync(selectedCharId);
                if (fetchedChar == null)
                {
                    client.SendChatMessage("Error retriving char. Bad char id?");
                    return;
                }
                var accData = client.GetAccount();
                if(accData == null) return;

                if (accData.ID != fetchedChar.CharOwnerID)
                {
                    client.SendChatMessage("That is not your character. Ban/Kick?");
                    return;
                }

                var app = await fetchedChar.GetAppearance();
                if (app != null) app.Apply(client);
                client.Transparency = 255;
            });
        }

        [RemoteEvent(Events.ClientToServer.Character.SubmitSpawnCharacter)]
        public void ClientEvent_SubmitSpawnCharacter(Client client, int selectedCharId)
        {
            if (!client.IsLoggedIn()) return;
            if (selectedCharId < 0) return;

            TaskManager.Run(client, async () =>
            {
                var chData = await Character.ReadAsync(selectedCharId);
                if (chData == null)
                {
                    client.SendChatMessage("Error retriving char. Bad char id?");
                    return;
                }

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
                client.Name = chData.CharacterName.Replace("_", " ");
                client.TriggerEvent(Events.ServerToClient.Character.EndCharSelector);

                // Invoke Character Spawn Listeners
                CharacterSpawn?.Invoke(client, EventArgs.Empty);
            });
        }

        [RemoteEvent(Events.ClientToServer.Character.SubmitInitialCharData)]
        public void ClientEvent_SubmitInitialCharData(Client client, string firstName, string lastName)
        {
            if(!client.IsLoggedIn()) return;

            if (!ValidateString(ValidationStrings.CharFirstName, firstName))
            {
                client.TriggerEvent(Events.ServerToClient.Character.DisplayCharError, "There is something wrong with that first name.");
                return;
            }

            if (!ValidateString(ValidationStrings.CharFirstName, lastName))
            {
                client.TriggerEvent(Events.ServerToClient.Character.DisplayCharError, "There is something wrong with that last name.");
                return;
            }

            TaskManager.Run(client, async () =>
            {
                var ch = await Character.ReadByKeyAsync(() => new Character().CharacterName, $"{firstName}_{lastName}");
                if (ch.Any())
                {
                    client.TriggerEvent(Events.ServerToClient.Character.DisplayCharError, "That character name already exists.");
                    return;
                }
                client.TriggerEvent(Events.ServerToClient.Character.StartCustomization);
            });
        }

        [RemoteEvent(Events.ClientToServer.Character.SubmitNewCharacter)]
        public void ClientEvent_SubmitNewCharacter(Client client, string dataAsJson)
        {
            if(!client.IsLoggedIn()) return;

            var newCharData = JsonConvert.DeserializeObject<AppearanceJson>(dataAsJson);

            if (!ValidateString(ValidationStrings.CharFirstName, newCharData.firstname))
            {
                client.TriggerEvent(Events.ServerToClient.Character.ResetCharCreation, "There is something wrong with that first name.");
                return;
            }

            if (!ValidateString(ValidationStrings.CharFirstName, newCharData.lastname))
            {
                client.TriggerEvent(Events.ServerToClient.Character.ResetCharCreation, "There is something wrong with that first name.");
                return;
            }

            TaskManager.Run(client, async () =>
            {
                var charName = $"{newCharData.firstname}_{newCharData.lastname}";

                var ch = await Character.ReadByKeyAsync(() => new Character().CharacterName, charName);
                if (ch.Any())
                {
                    client.TriggerEvent(Events.ServerToClient.Character.ResetCharCreation, "That character name already exists.");
                    return;
                }

                await Character.CreateNewAsync(client.GetAccount(), charName);
                var newChIEnumerable = await Character.ReadByKeyAsync(() => new Character().CharacterName, charName);
                var newCh = newChIEnumerable.First();

                var pedhash = newCharData.isMale ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01;
                var newChApp = new Appearance(pedhash, newCh);
                newChApp.Populate(newCharData);
                await Appearance.CreateAsync(newChApp);

                client.TriggerEvent(Events.ServerToClient.Character.SuccessCharCreation);
            });
        }

        [RemoteEvent(Events.ClientToServer.Character.TriggerCharSelection)]
        public void ClientEvent_TriggerCharSelection(Client client)
        {
            InitCharacterSelection(client);
        }

        [RemoteEvent(Events.ClientToServer.Character.RequestAliasInfo)]
        public void ClientEvent_RequestAliasInfo(Client client, int remoteid)
        {
            if(!client.IsLoggedIn() || !client.HasActiveChar()) return;
            var streamedClient = ClientMethods.FindClientByPlayerID(remoteid);
            if (!streamedClient.IsLoggedIn() || !streamedClient.HasActiveChar()) return;

            var chData = client.GetActiveChar();
            var chOtherData = streamedClient.GetActiveChar();

            var alias = chData.Aliases.First(i => i.AliasedID == chOtherData.ID);

            if (alias != null) client.TriggerEvent(Events.ServerToClient.Character.SetAliasInfo, $"{alias.AliasName} ({chOtherData.AltIdentifier.ToString()})", remoteid);
            else client.TriggerEvent(Events.ServerToClient.Character.SetAliasInfo, $"({chOtherData.AltIdentifier.ToString()})", remoteid);
        }

        private static void OnPlayerLogin(object source, EventArgs e)
        {
            var client = source as Client;
            if (client == null) return;
            if (!client.IsLoggedIn()) return;

            InitCharacterSelection(client);
        }
        private static void OnCharacterSpawn(object source, EventArgs e)
        {
            // Load Character Data
            var client = source as Client;
            if (client == null) return;
            if (!client.IsLoggedIn()) return;
            var chData = client.GetActiveChar();
            if (chData == null) return;

            TaskManager.Run(client, async () =>
            {
                chData.Appearance = await chData.GetAppearance();
                chData.Aliases = await chData.GetAliases();
            });

        }

        private static void InitCharacterSelection(Client client)
        {
            client.TriggerEvent(Events.ServerToClient.Character.InitCharSelector);
            client.ResetActiveChar();
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
                client.TriggerEvent(Events.ServerToClient.Character.RenderCharacterList, JsonConvert.SerializeObject(charDisplayList), acc.LastSpawnedCharId);
            });
        }
    }
}
