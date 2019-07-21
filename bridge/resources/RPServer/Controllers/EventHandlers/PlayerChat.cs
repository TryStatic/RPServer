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

            var acc = client.GetAccount();
            var sendMsg = $"{acc.Username}: {message}";

            var ch = client.GetActiveChar();
            if(client.GetActiveChar() != null) sendMsg = $"{client.GetActiveChar().CharacterName.Replace("_", " ")}: {message}";


            Logger.GetInstance().ChatLog(sendMsg);
            NAPI.ClientEvent.TriggerClientEventForAll("SendToChat", sendMsg);
        }
    }
}
