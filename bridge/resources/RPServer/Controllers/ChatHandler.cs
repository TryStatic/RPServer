using Google.Protobuf.Reflection;
using GTANetworkAPI;
using GTANetworkMethods;

namespace RPServer.Controllers
{
    internal static class ChatHandler
    {
        // TODO: Add commands and methods for chat related stuff (local/faction w/e chat etc)
        // TODO: This should work in combination with the PlayerChat Event Handler

        public static void SendNormalChat(Client client, string message)
        {
            NAPI.ClientEvent.TriggerClientEventInRange(client.Position, 10f, "SendToChat", message);
        }
    }
}
