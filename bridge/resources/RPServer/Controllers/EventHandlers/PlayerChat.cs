using System;
using System.Security;
using System.Text.RegularExpressions;
using GTANetworkAPI;
using RPServer.InternalAPI.Extensions;

namespace RPServer.Controllers.EventHandlers
{
    internal class PlayerChat : Script
    {
        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Client client, string message)
        {
            // Default Chat, called by the API
            if (!client.IsLoggedIn()) return;
            if (!client.HasActiveChar()) return;
        }

        public static string Filter(string message)
        {
            if (message == null) return null;
            if (message == "") return message;

            // Filter HTML
            message = SecurityElement.Escape(message);

            // Filter colors
            var matches = new Regex(@"(!{#[0-9A-F]{6}})+").Matches(message);
            foreach (Match m in matches)
                message = message.Remove(message.IndexOf(m.Value, StringComparison.OrdinalIgnoreCase), 10);

            return message;
        }
    }
}