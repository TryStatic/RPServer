using GTANetworkAPI;

namespace RPServer.Controllers
{
    internal class ChatHandler : Script
    {
        // TODO: Add commands and methods for chat related stuff (local/faction w/e chat etc)
        // TODO: This should work in combination with the PlayerChat Event Handler
        [Command("unfiltered", GreedyArg = true)]
        public void cmd_colorchar(Client client, string message)
        {
            NAPI.ClientEvent.TriggerClientEventForAll("SendToChat", message);
        }

        public static void SendNormalChat(Client client, string message)
        {
            NAPI.ClientEvent.TriggerClientEventInRange(client.Position, 10f, "SendNormalChat", message, client.Value);
            //NAPI.ClientEvent.TriggerClientEventInRange(client.Position, 10f, "SendToChat", message);
        }
    }
}
