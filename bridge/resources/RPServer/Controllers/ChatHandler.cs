using GTANetworkAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;

namespace RPServer.Controllers
{
    internal class ChatHandler : Script
    {
        // TODO: Add commands and methods for chat related stuff (local/faction w/e chat etc)
        // TODO: This should work in combination with the PlayerChat Event Handler
        [Command("unfiltered", GreedyArg = true)]
        public void cmd_colorchar(Client client, string message)
        {
            NAPI.ClientEvent.TriggerClientEventForAll("PushToChatUnfiltered", message);
        }

        [RemoteEvent(Shared.Events.ClientToServer.Chat.SubmitLocalNormalChatMessage)]
        public void ClientEvent_OnSubmitLocalNormalChatMessage(Client client, string message)
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;

            NAPI.ClientEvent.TriggerClientEventInRange(client.Position, 10f, Shared.Events.ServerToClient.Chat.PushToChat, message, client.Value);
            Logger.GetInstance().ChatLog($"{client.GetActiveChar().CharacterName} says: {message}");
        }

        [RemoteEvent(Shared.Events.ClientToServer.Chat.SubmitLocalShoutChatMessage)]
        public void ClientEvent_OnSubmitLocalShoutChatMessage(Client client, string message)
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;

            NAPI.ClientEvent.TriggerClientEventInRange(client.Position, 20f, Shared.Events.ServerToClient.Chat.PushToChat, message, client.Value);
            Logger.GetInstance().ChatLog($"{client.GetActiveChar().CharacterName} shout: {message}");
        }
    }
}
