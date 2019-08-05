using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using GTANetworkAPI;
using Newtonsoft.Json;
using RPServer.Controllers.Util;
using RPServer.InternalAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Models;
using RPServer.Resource;
using RPServer.Util;
using Shared.Data;
using Events = Shared.Events;
using static RPServer.Controllers.Util.DataValidator;

namespace RPServer.Controllers
{
    public delegate void OnCharacterSpawnDelegate(object source, EventArgs e);
    public delegate void OnCharacterDespawnDelegate(object source, EventArgs e);

    internal class CharacterHandler : Script
    {
        public static event OnCharacterSpawnDelegate CharacterSpawn;
        public static event OnCharacterSpawnDelegate CharacterDespawn;

        public CharacterHandler()
        {
            AccountManager.PlayerLogin += OnPlayerLogin;
            CharacterHandler.CharacterSpawn += OnCharacterSpawn;
            CharacterHandler.CharacterDespawn += OnCharacterDespawn;
        }

        [Command(CmdStrings.CMD_Stats, GreedyArg = true)]
        public void CMD_Stats(Client client, string args = "")
        {
            var accdata = client.GetAccount();
            var chdata = client.GetActiveChar();
            var timeplayed = TimeSpan.FromMinutes(chdata.MinutesPlayed);
            ChatHandler.SendClientMessage(client, $"\t<!S>---[{DateTime.Now:D}]---");
            ChatHandler.SendClientMessage(client, $"\t<!S>Username: {accdata.Username} | ActiveChar: {chdata.CharacterName} | Email: {accdata.EmailAddress} | AdminLevel: {accdata.AdminLevel}");
            ChatHandler.SendClientMessage(client, $"\t<!S>2FAbyEmail: { accdata.HasEnabledTwoStepByEmail} | 2FAbyGoogleAuth: { accdata.Is2FAbyGAEnabled()} | LastIP: {accdata.LastIP}");
            ChatHandler.SendClientMessage(client, $"\t<!S>Nickname: {accdata.ForumName} | ForumName: {accdata.ForumName} | TimePlayed: {timeplayed.Hours}h:{timeplayed.Minutes}m");
            ChatHandler.SendClientMessage(client, $"\t<!S>Creation: {accdata.CreationDate:MM/dd/yyyy} | LastLogin: {accdata.LastLoginDate:MM/dd/yyyy}");
            ChatHandler.SendClientMessage(client, $"\t<!S>-----------------------------------------------------------------------");

        }

        [Command(CmdStrings.CMD_ChangeChar)]
        public void CMD_ChangeChar(Client client)
        { // Temporary (?)
            if (!client.HasActiveChar())
            {
                client.SendChatMessage("You are not spawned yet.");
                return;
            }
            
            CharacterDespawn?.Invoke(client, EventArgs.Empty);

            InitCharacterSelection(client);
        }
        
        [Command(CmdStrings.CMD_Alias, GreedyArg = true)]
        public void CMD_Alias(Client client, string args = "")
        {
            var cmdParser = new CommandParser(args);

            if(!client.IsLoggedIn() || !client.HasActiveChar()) return;

            if (!cmdParser.HasNextToken())
            {
                ChatHandler.SendCommandUsageText(client, "/alias [PartOfName/ID] [optional: AliasText]");
                return;
            }

            var identifier = cmdParser.GetNextToken();
            var otherClient = ClientMethods.FindClient(identifier);
            if (otherClient == null)
            {
                ChatHandler.SendCommandErrorText(client, "Couldn't find that player.");
                return;
            }

            if (!otherClient.IsLoggedIn())
            {
                ChatHandler.SendCommandErrorText(client, "That player is not logged in");
                return;
            }

            if (!otherClient.HasActiveChar())
            {
                ChatHandler.SendCommandErrorText(client, "That player is not spawned.");
                client.SendChatMessage("That player is not spawned.");
                return;
            }

            // TODO: DISTANCE CHECK

            var chData = client.GetActiveChar();
            var chOtherData = otherClient.GetActiveChar();

            if (!cmdParser.HasNextToken())
            {
                var alias = chData.Aliases.FirstOrDefault(i => i.CharID == chData.ID && i.AliasedID == chOtherData.ID);
                if (alias == null)
                {
                    ChatHandler.SendCommandErrorText(client, "You haven't set an alias for that player.");
                    return;
                }
                chData.Aliases.Remove(alias);
                ClientEvent_RequestAliasInfo(client, otherClient.Value);
                ChatHandler.SendCommandSuccessText(client, "Alias removed.");
                return;
            }
            else
            {
                var aliasText = cmdParser.GetNextToken();
                if (aliasText.Length > 20)
                {
                    ChatHandler.SendCommandErrorText(client, "Alias text is too long. (max: 20chars)");
                    return;
                }
                var exist = chData.Aliases.FirstOrDefault(i => i.CharID == chData.ID && i.AliasedID == chOtherData.ID);
                if (exist != null)
                {
                    exist.AliasName = aliasText;
                }
                else
                {
                    chData.Aliases.Add(new Alias(chData, chOtherData, aliasText));
                }

                ClientEvent_RequestAliasInfo(client, otherClient.Value);
                ChatHandler.SendCommandSuccessText(client, "Alias set.");
            }
        }

        [RemoteEvent(Events.ClientToServer.Character.ApplyCharacterEditAnimation)]
        public void ClientEvent_ApplyCharacterEditAnimation(Client client) => client.PlayAnimation("missbigscore2aleadinout@ig_7_p2@bankman@", "leadout_waiting_loop", 1);

        [RemoteEvent(Events.ClientToServer.Character.SubmitCharacterSelection)]
        public async void ClientEvent_SubmitCharacterSelection(Client client, int selectedCharId)
        {
            if (!client.IsLoggedIn()) return;
            if (selectedCharId < 0) return;

            var fetchedChar = await CharacterModel.ReadAsync(selectedCharId);
            if (fetchedChar == null)
            {
                client.SendChatMessage("Error retriving char. Bad char id?");
                return;
            }
            var accData = client.GetAccount();
            if (accData == null) return;

            if (accData.ID != fetchedChar.CharOwnerID)
            {
                client.SendChatMessage("That is not your character. Ban/Kick?");
                return;
            }

            var app = (await AppearanceModel.ReadByKeyAsync(() => AppearanceModel.Mock.CharacterID, fetchedChar.ID)).FirstOrDefault();
            if (app != null) app.Apply(client);
            client.Transparency = 255;
        }

        [RemoteEvent(Events.ClientToServer.Character.SubmitSpawnCharacter)]
        public async void ClientEvent_SubmitSpawnCharacter(Client client, int selectedCharId)
        {
            if (!client.IsLoggedIn()) return;
            if (selectedCharId < 0) return;

            var chData = await CharacterModel.ReadAsync(selectedCharId);
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
            client.Name = chData.CharacterName;
            client.TriggerEvent(Events.ServerToClient.Character.EndCharSelector);

            // Invoke Character Spawn Listeners
            CharacterSpawn?.Invoke(client, EventArgs.Empty);
        }

        [RemoteEvent(Events.ClientToServer.Character.SubmitInitialCharData)]
        public async void ClientEvent_SubmitInitialCharData(Client client, string firstName, string lastName)
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

            var ch = await CharacterModel.ReadByKeyAsync(() => CharacterModel.Mock.CharacterName, $"{firstName}_{lastName}");
            if (ch.Any())
            {
                client.TriggerEvent(Events.ServerToClient.Character.DisplayCharError, "That character name already exists.");
                return;
            }
            client.TriggerEvent(Events.ServerToClient.Character.StartCustomization);
        }

        [RemoteEvent(Events.ClientToServer.Character.SubmitNewCharacter)]
        public async void ClientEvent_SubmitNewCharacter(Client client, string dataAsJson)
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

            var charName = $"{newCharData.firstname}_{newCharData.lastname}";

            var ch = await CharacterModel.ReadByKeyAsync(() => CharacterModel.Mock.CharacterName, charName);
            if (ch.Any())
            {
                client.TriggerEvent(Events.ServerToClient.Character.ResetCharCreation, "That character name already exists.");
                return;
            }

            await CharacterModel.CreateNewAsync(client.GetAccount(), charName);
            var newChIEnumerable = await CharacterModel.ReadByKeyAsync(() => CharacterModel.Mock.CharacterName, charName);
            var newCh = newChIEnumerable.First();

            var pedhash = newCharData.isMale ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01;
            var newChApp = new AppearanceModel(pedhash, newCh);
            newChApp.Populate(newCharData);
            await AppearanceModel.CreateAsync(newChApp);

            client.TriggerEvent(Events.ServerToClient.Character.SuccessCharCreation);
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

            var alias = chData.Aliases.FirstOrDefault(i => i.AliasedID == chOtherData.ID);

            if (alias != null) client.TriggerEvent(Events.ServerToClient.Character.SetAliasInfo, $"{alias.AliasName} ({streamedClient.Value})", remoteid);
            else client.TriggerEvent(Events.ServerToClient.Character.SetAliasInfo, $"({streamedClient.Value})", remoteid);
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

            chData.ReadAllData().GetAwaiter().GetResult();
            var saveDataTimer = new Timer(OnMinuteSpent, client, 1000 * 60 * 5, 1000 * 60 * 5); // 5 minutes
            var MinutesPlayedTimer = new Timer(OnSaveData, client, 1000 * 60, 1000 * 60); // 1 minute
            client.SetData(DataKey.TimerPlayerSaveData, saveDataTimer);
            client.SetData(DataKey.TimerPlayerMinuteSpent, MinutesPlayedTimer);
        }

        private void OnCharacterDespawn(object source, EventArgs e)
        {
            var client = source as Client;
            if (client == null) return;

            DespawnCharacter(client);
        }

        public static void DespawnCharacter(Client client)
        {
            var ch = client.GetActiveChar();
            ch?.SaveAllData();
            client.ResetActiveChar();

#if DEBUG
            ChatHandler.SendClientMessage(client, "!{#FF0000}[DEBUG-OnCharDespawn]: !{#FFFFFF}Disposing character timers.");
#endif
            Timer saveDataTimer = client.GetData("SAVE_DATA_TIMER");
            Timer minutesPlayedTimer = client.GetData("MINUTES_PLAYED_TIMER");

            if (saveDataTimer != null)
            {
                saveDataTimer.Dispose();
                client.ResetData(DataKey.TimerPlayerSaveData);
            }

            if (minutesPlayedTimer != null)
            {
                minutesPlayedTimer.Dispose();
                client.ResetData(DataKey.TimerPlayerMinuteSpent);
            }

        }

        private static void OnMinuteSpent(object state)
        {
            var client = state as Client;
            if (client == null) return;

            if (!client.IsLoggedIn() || !client.HasActiveChar())
            {
                Logger.GetInstance().ServerError("OnMinuteSpent called w/o an active char.");
                return;
            }
            var chData = client.GetActiveChar();

            chData.MinutesPlayed++;

            if (chData.MinutesPlayed % 60 == 0)
            {
                ChatHandler.SendClientMessage(client, "Payday!");
            }

        }

        private static async void OnSaveData(object state)
        {
            var client = state as Client;

            if (!client.IsLoggedIn() || !client.HasActiveChar())
            {
                Logger.GetInstance().ServerError("OnSaveData called w/o an active char.");
                return;
            }
            await client.GetActiveChar().SaveAllData();
            await client.GetAccount().UpdateAsync();
        }

        private static async void InitCharacterSelection(Client client)
        {
            client.TriggerEvent(Events.ServerToClient.Character.InitCharSelector);
            client.ResetActiveChar();
            client.Dimension = (uint)client.Value + 1500;

            var acc = client.GetAccount();
            var charList = await CharacterModel.FetchAllAsync(acc);
            var charDisplayList = new List<CharDisplay>();
            foreach (var c in charList)
            {
                charDisplayList.Add(new CharDisplay(c.ID, c.CharacterName));
            }
            client.TriggerEvent(Events.ServerToClient.Character.RenderCharacterList, JsonConvert.SerializeObject(charDisplayList), acc.LastSpawnedCharId);
        }
    }
}
