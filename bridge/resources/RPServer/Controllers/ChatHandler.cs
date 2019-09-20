using System;
using System.Security;
using System.Text.RegularExpressions;
using GTANetworkAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Resource;
using RPServer.Util;
using RPServerClient.Chat.Util;
using Shared.Data;
using Chat = Shared.Events.ServerToClient.Chat;

namespace RPServer.Controllers
{
    internal class ChatHandler : Script
    {
        [Command(CommandHandler.Chat_OOC_Text, Alias = CommandHandler.Chat_OOC_Text_Alias, GreedyArg = true)]
        public void CMD_OOC(Client client, string message = "")
        {
            if(!CommandHandler.Chat_OOC.IsAuthorized(client)) return;

            if (message == "")
            {
                SendCommandUsageText(client, "/o [Global OOC]");
                return;
            }

            message = EscapeHTML(message);
            message = RemoveColors(message);

            SendGlobalOOCMesssage(client, message);
        }

        [Command(CommandHandler.Chat_B_Text, GreedyArg = true)]
        public void CMD_B(Client client, string message = "")
        {
            if (!CommandHandler.Chat_B.IsAuthorized(client)) return;

            if (message == "")
            {
                SendCommandUsageText(client, "/b [Local OOC]");
                return;
            }

            message = EscapeHTML(message);
            message = RemoveColors(message);
            SendLocalOOCMesssage(client, Shared.Data.Chat.NormalChatMaxDistance, message);
        }

        [Command(CommandHandler.Chat_Me_Text, GreedyArg = true)]
        public void CMD_Me(Client client, string message = "")
        {
            if (!CommandHandler.Chat_Me.IsAuthorized(client)) return;

            if (message == "")
            {
                SendCommandUsageText(client, "/me [action]");
                return;
            }
            message = EscapeHTML(message);
            message = RemoveColors(message);
            SendActionMessage(client, Shared.Data.Chat.NormalChatMaxDistance, message);
        }

        [Command(CommandHandler.Chat_Do_Text, GreedyArg = true)]
        public void CMD_Do(Client client, string message = "")
        {
            if (!CommandHandler.Chat_Do.IsAuthorized(client)) return;

            if (message == "")
            {
                SendCommandUsageText(client, "/do [description]");
                return;
            }

            message = EscapeHTML(message);
            message = RemoveColors(message);
            SendDescriptionMessage(client, Shared.Data.Chat.NormalChatMaxDistance, message);
        }


        [RemoteEvent(Shared.Events.ClientToServer.Chat.SubmitChatMessage)]
        public void OnSubmitChatMessage(Client client, string playerText, int chatModeAsInt)
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;

            var chatMode = (ChatMode) chatModeAsInt;
            playerText = EscapeHTML(playerText);
            playerText = RemoveColors(playerText);

            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                string textColor;
                switch (chatMode)
                {
                    case ChatMode.Low:
                        if (client.Position.DistanceTo(p.Position) > Shared.Data.Chat.LowChatMaxDistance)
                            continue;

                        // Add a full stop at the end of the message if needed
                        if (playerText[playerText.Length - 1] != '.') playerText += ".";

                        textColor = GetLocalChatMessageColor(client, p, Shared.Data.Chat.LowChatMaxDistance);
                        NAPI.ClientEvent.TriggerClientEvent(p, Chat.PushChatMessage, $" says: {playerText}",
                            client.Value, $"{Colors.COLOR_GRAD3}[Low]{textColor} ");
                        break;
                    case ChatMode.Normal:
                        if (client.Position.DistanceTo(p.Position) > Shared.Data.Chat.NormalChatMaxDistance)
                            continue;

                        // Add a full stop at the end of the message if needed
                        if (playerText[playerText.Length - 1] != '.') playerText += ".";

                        textColor = GetLocalChatMessageColor(client, p, Shared.Data.Chat.NormalChatMaxDistance);
                        NAPI.ClientEvent.TriggerClientEvent(p, Chat.PushChatMessage, $" says: {playerText}",
                            client.Value, textColor);
                        break;
                    case ChatMode.Shout:
                        if (client.Position.DistanceTo(p.Position) > Shared.Data.Chat.ShoutChatMaxDistance)
                            continue;

                        // Add an exclamation mark at the end of the message if needed
                        if (playerText[playerText.Length - 1] != '!') playerText += "!";

                        textColor = GetLocalChatMessageColor(client, p, Shared.Data.Chat.ShoutChatMaxDistance);
                        NAPI.ClientEvent.TriggerClientEvent(p, Chat.PushChatMessage, $" shouts: {playerText}",
                            client.Value, textColor);
                        break;
                    default:
                        NAPI.Util.ConsoleOutput("Error OnSubmitChatMessage while switching though chatmodes.");
                        return;
                }
            }

            Logger.GetInstance().ChatLog($"{client.GetActiveChar().CharacterName}: {playerText}");
        }


        private string GetLocalChatMessageColor(Client client, Client other, float maxDistance)
        {
            var distance = client.Position.DistanceTo(other.Position);

            if (distance < maxDistance / 16) return Colors.COLOR_WHITE;
            if (distance < maxDistance / 8) return Colors.COLOR_GRAD1;
            if (distance < maxDistance / 4) return Colors.COLOR_GRAD2;
            if (distance < maxDistance / 2) return Colors.COLOR_GRAD3;
            if (distance < maxDistance) return Colors.COLOR_GRAD4;
            return Colors.COLOR_GRAD5;
        }

        private string RemoveColors(string message)
        {
            var matches = new Regex(@"(!{#[0-9A-F]{6}})+").Matches(message);
            foreach (Match m in matches)
                message = message.Remove(message.IndexOf(m.Value, StringComparison.OrdinalIgnoreCase), 10);
            return message;
        }

        private static string EscapeHTML(string message)
        {
            return SecurityElement.Escape(message);
        }

        internal static void SendCommandUsageText(Client client, string usageText)
        {
            client.TriggerEvent(Chat.PushChatMessageUnfiltered,
                EscapeHTML($"{Colors.COLOR_GRAD3}[Usage]: {Colors.COLOR_GRAD1}{usageText}"));
        }

        internal static void SendCommandErrorText(Client client, string errorText)
        {
            client.TriggerEvent(Chat.PushChatMessageUnfiltered,
                EscapeHTML($"{Colors.COLOR_YELLOW}<!> {Colors.COLOR_WHITE}{errorText}"));
        }

        internal static void SendCommandSuccessText(Client client, string text)
        {
            client.TriggerEvent(Chat.PushChatMessageUnfiltered, EscapeHTML($"{Colors.COLOR_GREEN}<!> {Colors.COLOR_WHITE}{text}"));
        }

        internal static void SendClientMessage(Client client, string message)
        {
            client.TriggerEvent(Chat.PushChatMessageUnfiltered, EscapeHTML(message));
        }

        internal static void SendClientMessageHTML(Client client, string message)
        {
            client.TriggerEvent(Chat.PushChatMessageUnfiltered, message);
        }

        /// <summary>
        /// Sends a /me type of message
        /// </summary>
        /// <param name="client">The source (sender)</param>
        /// <param name="distance">The distance</param>
        /// <param name="message">The message</param>
        public static void SendActionMessage(Client client, float distance, string message)
        {
            if (message[message.Length - 1] != '.') message += ".";
            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (client.Position.DistanceTo(p.Position) > distance) continue;
                NAPI.ClientEvent.TriggerClientEvent(p, Chat.PushActionMessage, message, client.Value, Colors.COLOR_PURPLE);
            }
        }

        public void SendDescriptionMessage(Client client, float distance, string message)
        {
            if (message[message.Length - 1] != '.') message += ".";
            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (client.Position.DistanceTo(p.Position) > distance) continue;

                NAPI.ClientEvent.TriggerClientEvent(p, Chat.PushDescriptionMessage, message, client.Value, Colors.COLOR_PURPLE);
            }
        }

        public void SendLocalOOCMesssage(Client client, float distance, string message)
        {
            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (client.Position.DistanceTo(p.Position) > Shared.Data.Chat.NormalChatMaxDistance) continue;

                var color = GetLocalChatMessageColor(client, p, Shared.Data.Chat.NormalChatMaxDistance);
                NAPI.ClientEvent.TriggerClientEvent(p, Chat.PushChatMessage, $": (( {message} ))", client.Value, color);
            }
        }

        public void SendGlobalOOCMesssage(Client client, string message)
        {
            var playerName = string.IsNullOrEmpty(client.GetAccount().NickName)
                ? client.GetActiveChar().CharacterName
                : client.GetAccount().NickName;

            NAPI.ClientEvent.TriggerClientEventForAll(Chat.PushChatMessageUnfiltered,
                $"{Colors.COLOR_LIGHTBLUE}(( [O] {playerName}: {message} ))");
        }
    }
}