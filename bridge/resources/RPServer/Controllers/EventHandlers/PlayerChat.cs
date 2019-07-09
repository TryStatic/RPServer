using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Controllers.EventHandlers
{
    internal class PlayerChat : Script
    {
        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Client client, string message)
        {
            if(!client.IsLoggedIn()) return;

            var acc = client.GetAccountData();
            var sendMsg = $"{acc.Username}: {message}";

            Logger.GetInstance().ChatLog(sendMsg);
            NAPI.ClientEvent.TriggerClientEventForAll("SendToChat", sendMsg);
        }
    }
}
