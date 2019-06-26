using GTANetworkAPI;

namespace RPServer.Chat
{
    class Chat : Script
    {
        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Client client, string message)
        {
            client.TriggerEvent("SendToChat", message);
        }
    }
}
