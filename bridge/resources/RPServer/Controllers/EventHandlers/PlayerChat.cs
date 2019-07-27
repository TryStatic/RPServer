using System;
using System.Text.RegularExpressions;
using GTANetworkAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;

namespace RPServer.Controllers.EventHandlers
{
    internal class PlayerChat : Script
    {
        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Client client, string message)
        { // Default Chat
            if (!client.IsLoggedIn()) return;
            if (!client.HasActiveChar()) return;

            message = Filter(message);

            var sendMsg = $"{client.GetActiveChar().CharacterName}: {message}";
            Logger.GetInstance().ChatLog(sendMsg);

            ChatHandler.SendNormalChat(client, sendMsg);
        }

        public string Filter(string message)
        {
            if (message == null) return null;
            if (message == "") return message;

            message = System.Security.SecurityElement.Escape(message);
            var matches = new Regex(@"(!{#[0-9A-F]{6}})+").Matches(message);
            foreach (Match m in matches) message = message.Remove(message.IndexOf(m.Value, StringComparison.OrdinalIgnoreCase), 10);

            return message;
        }
    }
}
