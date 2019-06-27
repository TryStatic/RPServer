using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Chat
{
    class Chat : Script
    {
        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Client client, string message)
        {
            if(!client.IsLoggedIn()) return;
            var acc = client.GetAccountData();
            var sendMsg = $"{acc.Username}: {message}";
            NAPI.ClientEvent.TriggerClientEventForAll("SendToChat", sendMsg);

        }
    }
}
