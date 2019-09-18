using System;
using System.Collections.Generic;
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
using Shared.Events.ClientToServer;
using static RPServer.Controllers.Util.DataValidator;

namespace RPServer.Controllers
{
    public delegate void OnCharacterSpawnDelegate(object source, EventArgs e);

    public delegate void OnCharacterDespawnDelegate(object source, EventArgs e);

    internal class CharacterHandler : Script
    {
        public CharacterHandler()
        {
            AccountManager.PlayerLogin += OnPlayerLogin;
            CharacterSpawn += OnCharacterSpawn;
            CharacterDespawn += OnCharacterDespawn;
        }

        public static event OnCharacterSpawnDelegate CharacterSpawn;
        public static event OnCharacterSpawnDelegate CharacterDespawn;

        [Command(CommandHandler.StatsText, GreedyArg = true)]
        public void CMD_Stats(Client client, string args = "")
        {
            if(!CommandHandler.Stats.IsAuthorized(client)) return;

            var usedExpand = false;
            var cmdParser = new CommandParser(args);
            if (cmdParser.HasNextToken() && cmdParser.GetNextToken() == "expand")
            { // used /stats expand
                usedExpand = true;
            }

            var acc = client.GetAccount();
            var ch = client.GetActiveChar();

            // Header
            var header = $"{Colors.COLOR_GRAD4}--------------------------------[{Colors.COLOR_ORANGE}{ch.CharacterName}{Colors.COLOR_GRAD4} ({Colors.COLOR_ORANGE}{acc.Username}{Colors.COLOR_GRAD4}) @ {Colors.COLOR_WHITE}{DateTime.Now:MM/dd/yyyy HH:mm:ss}{Colors.COLOR_GRAD4}]--------------------------------";

            // Account Details
            var verifiedEmail = acc.HasVerifiedEmail() ? $"{Colors.COLOR_GREEN}Verified" : $"{Colors.COLOR_RED}Unverified";
            var nickName = string.IsNullOrEmpty(acc.NickName) ? $"{Colors.COLOR_WHITE}<i>None</i>" : $"{Colors.COLOR_WHITE}{acc.NickName}";
            var forumName = string.IsNullOrEmpty(acc.NickName) ? $"{Colors.COLOR_WHITE}<i>None</i>" : $"{Colors.COLOR_WHITE}{acc.ForumName}";
            var accdetails = $"{Colors.COLOR_GRAD4}Email: {Colors.COLOR_WHITE}{acc.EmailAddress}{Colors.COLOR_GRAD4} ({verifiedEmail}{Colors.COLOR_GRAD4}) | Nickname: {nickName}{Colors.COLOR_GRAD4} | Forumname: {forumName}";

            // Security
            var twoFactorEmail = acc.Is2FAbyEmailEnabled() ? $"{Colors.COLOR_GREEN}Enabled" : $"{Colors.COLOR_RED}Disabled";
            var twoFactorGA = acc.Is2FAbyGAEnabled() ? $"{Colors.COLOR_GREEN}Enabled" : $"{Colors.COLOR_RED}Disabled";
            var twoFactorAuth = $"{Colors.COLOR_GRAD4}Two Factor Authentication: {Colors.COLOR_WHITE}By Email: {twoFactorEmail}{Colors.COLOR_GRAD4} | {Colors.COLOR_WHITE}By Google Authenticator App: {twoFactorGA}";

            // Assets
            var assets = $"{Colors.COLOR_GRAD4}Vehicles: [{Colors.COLOR_WHITE}{ch.Vehicles.Count}/X{Colors.COLOR_GRAD4}] | <i>Add Rest of Assets Information Summary</i>";

            // Paycheck
            var timeplayed = TimeSpan.FromMinutes(ch.MinutesPlayed);
            var nextpaycheck = 60 - timeplayed.Minutes;
            var timing = $"{Colors.COLOR_GRAD4}Character Playtime: {Colors.COLOR_WHITE}{timeplayed.Hours}h:{timeplayed.Minutes}m{Colors.COLOR_GRAD4} | Next PayCheck in: {Colors.COLOR_WHITE}{nextpaycheck}m";

            // Footer
            string footer;
            if (!usedExpand) footer = $"{Colors.COLOR_GRAD4}-----------------------> For more detailed information please use /stats expand <-----------------------";
            else footer = $"{Colors.COLOR_GRAD4}-------------------------------------------------------------------------------------------------------------------------------";

            ChatHandler.SendClientMessageHTML(client, header);
            ChatHandler.SendClientMessageHTML(client, accdetails);
            ChatHandler.SendClientMessageHTML(client, twoFactorAuth);
            ChatHandler.SendClientMessageHTML(client, $"{Colors.COLOR_GRAD5}<i>TODO: Add Summary of Financial Information for current character</i>");
            ChatHandler.SendClientMessageHTML(client, timing);
            ChatHandler.SendClientMessageHTML(client, $"{Colors.COLOR_GRAD5}<i>TODO: Add Job and Faction Information Summary</i>");
            ChatHandler.SendClientMessageHTML(client, assets);
            ChatHandler.SendClientMessageHTML(client, footer);

            if (usedExpand)
            {
                ChatHandler.SendClientMessageHTML(client, "<i>TODO: Display CEF page with detailed account statistics.");

            }
        }

        [Command(CommandHandler.ChangeCharText)]
        public void CMD_ChangeChar(Client client)
        {
            if (!CommandHandler.ChangeChar.IsAuthorized(client)) return;

            CharacterDespawn?.Invoke(client, EventArgs.Empty);
            InitCharacterSelection(client);
        }

        [Command(CommandHandler.AliasText, GreedyArg = true)]
        public void CMD_Alias(Client client, string args = "")
        {
            if (!CommandHandler.Alias.IsAuthorized(client)) return;

            var cmdParser = new CommandParser(args);

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
                return;
            }

            if (client.Position.DistanceTo(otherClient.Position) > Shared.Data.Chat.NormalChatMaxDistance)
            {
                ChatHandler.SendCommandErrorText(client, "That player is too far away.");
                return;
            }

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
                    exist.AliasName = aliasText;
                else
                    chData.Aliases.Add(new Alias(chData, chOtherData, aliasText));

                ClientEvent_RequestAliasInfo(client, otherClient.Value);
                ChatHandler.SendCommandSuccessText(client, "Alias set.");
            }
        }

        [RemoteEvent(Character.SubmitCharacterSelection)]
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

            var app = (await AppearanceModel.ReadByKeyAsync(() => AppearanceModel.Mock.CharacterID, fetchedChar.ID))
                .FirstOrDefault();
            if (app != null) app.Apply(client);
            client.Transparency = 255;
        }

        [RemoteEvent(Character.SubmitSpawnCharacter)]
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
            client.SetActiveChar(chData);
            client.Name = chData.CharacterName;
            ChatHandler.SendClientMessage(client, "Teleporting you to your last known position.");
            client.Position = new Vector3(chData.LastX, chData.LastY, chData.LastZ);
            client.TriggerEvent(Shared.Events.ServerToClient.Character.EndCharSelector);

            // Invoke Character Spawn Listeners
            CharacterSpawn?.Invoke(client, EventArgs.Empty);
        }

        [RemoteEvent(Character.SubmitInitialCharData)]
        public async void ClientEvent_SubmitInitialCharData(Client client, string firstName, string lastName)
        {
            if (!client.IsLoggedIn()) return;

            if (!ValidateString(ValidationStrings.CharFirstName, firstName))
            {
                client.TriggerEvent(Shared.Events.ServerToClient.Character.DisplayCharError,
                    "There is something wrong with that first name.");
                return;
            }

            if (!ValidateString(ValidationStrings.CharFirstName, lastName))
            {
                client.TriggerEvent(Shared.Events.ServerToClient.Character.DisplayCharError,
                    "There is something wrong with that last name.");
                return;
            }

            var ch = await CharacterModel.ReadByKeyAsync(() => CharacterModel.Mock.CharacterName,
                $"{firstName}_{lastName}");
            if (ch.Any())
            {
                client.TriggerEvent(Shared.Events.ServerToClient.Character.DisplayCharError,
                    "That character name already exists.");
                return;
            }

            client.TriggerEvent(Shared.Events.ServerToClient.Character.StartCustomization);
        }

        [RemoteEvent(Character.SubmitNewCharacter)]
        public async void ClientEvent_SubmitNewCharacter(Client client, string dataAsJson)
        {
            if (!client.IsLoggedIn()) return;

            var newCharData = JsonConvert.DeserializeObject<AppearanceJson>(dataAsJson);

            if (!ValidateString(ValidationStrings.CharFirstName, newCharData.firstname))
            {
                client.TriggerEvent(Shared.Events.ServerToClient.Character.ResetCharCreation,
                    "There is something wrong with that first name.");
                return;
            }

            if (!ValidateString(ValidationStrings.CharFirstName, newCharData.lastname))
            {
                client.TriggerEvent(Shared.Events.ServerToClient.Character.ResetCharCreation,
                    "There is something wrong with that first name.");
                return;
            }

            var charName = $"{newCharData.firstname}_{newCharData.lastname}";

            var ch = await CharacterModel.ReadByKeyAsync(() => CharacterModel.Mock.CharacterName, charName);
            if (ch.Any())
            {
                client.TriggerEvent(Shared.Events.ServerToClient.Character.ResetCharCreation,
                    "That character name already exists.");
                return;
            }

            await CharacterModel.CreateNewAsync(client.GetAccount(), charName);
            var newChIEnumerable =
                await CharacterModel.ReadByKeyAsync(() => CharacterModel.Mock.CharacterName, charName);
            var newCh = newChIEnumerable.First();

            var pedhash = newCharData.isMale ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01;
            var newChApp = new AppearanceModel(pedhash, newCh);
            newChApp.Populate(newCharData);
            await AppearanceModel.CreateAsync(newChApp);

            client.TriggerEvent(Shared.Events.ServerToClient.Character.SuccessCharCreation);
        }

        [RemoteEvent(Character.TriggerCharSelection)]
        public void ClientEvent_TriggerCharSelection(Client client)
        {
            InitCharacterSelection(client);
        }

        [RemoteEvent(Character.RequestAliasInfo)]
        public void ClientEvent_RequestAliasInfo(Client client, int remoteid)
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;
            var streamedClient = ClientMethods.FindClientByPlayerID(remoteid);
            if (!streamedClient.IsLoggedIn() || !streamedClient.HasActiveChar()) return;

            var chData = client.GetActiveChar();
            var chOtherData = streamedClient.GetActiveChar();

            var alias = chData.Aliases.FirstOrDefault(i => i.AliasedID == chOtherData.ID);

            if (alias != null)
                client.TriggerEvent(Shared.Events.ServerToClient.Character.SetAliasInfo,
                    $"{alias.AliasName} ({streamedClient.Value})", remoteid);
            else
                client.TriggerEvent(Shared.Events.ServerToClient.Character.SetAliasInfo, $"({streamedClient.Value})",
                    remoteid);
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
            var saveDataTimer = new Timer(OnSaveData, client, 1000 * 60 * 5, 1000 * 60 * 5); // 5 minutes
            var MinutesPlayedTimer = new Timer(OnMinuteSpent, client, 1000 * 60, 1000 * 60); // 1 minute
            client.SetData(DataKey.TimerPlayerSaveData, saveDataTimer);
            client.SetData(DataKey.TimerPlayerMinuteSpent, MinutesPlayedTimer);
        }

        private static void OnCharacterDespawn(object source, EventArgs e)
        {
            var client = source as Client;
            if (client == null) return;

            DespawnCharacter(client);
        }

        public static void DespawnCharacter(Client client)
        {
            var ch = client.GetActiveChar();
            ch?.SaveAllData(client);
            client.ResetActiveChar();

#if DEBUG
            ChatHandler.SendClientMessage(client,
                "!{#FF0000}[DEBUG-OnCharDespawn]: !{#FFFFFF}Disposing character timers.");
#endif
            Timer saveDataTimer = client.GetData(DataKey.TimerPlayerSaveData);
            Timer minutesPlayedTimer = client.GetData(DataKey.TimerPlayerMinuteSpent);

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

            if (chData.MinutesPlayed % 60 == 0) ChatHandler.SendClientMessage(client, "Payday!");
        }

        private static async void OnSaveData(object state)
        {
            var client = state as Client;

            if (!client.IsLoggedIn() || !client.HasActiveChar())
            {
                Logger.GetInstance().ServerError("OnSaveData called w/o an active char.");
                return;
            }

            await client.GetActiveChar().SaveAllData(client);
            await client.GetAccount().UpdateAsync();
        }

        private static async void InitCharacterSelection(Client client)
        {
            client.TriggerEvent(Shared.Events.ServerToClient.Character.InitCharSelector);
            client.ResetActiveChar();
            client.Dimension = (uint) client.Value + 1500;

            var acc = client.GetAccount();
            var charList = await CharacterModel.FetchAllAsync(acc);
            var charDisplayList = new List<CharDisplay>();
            foreach (var c in charList) charDisplayList.Add(new CharDisplay(c.ID, c.CharacterName));
            client.TriggerEvent(Shared.Events.ServerToClient.Character.RenderCharacterList,
                JsonConvert.SerializeObject(charDisplayList), acc.LastSpawnedCharId);
        }
    }
}