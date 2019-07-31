using GTANetworkAPI;
using RPServer.InternalAPI;
using RPServer.InternalAPI.Extensions;

namespace RPServer.Controllers
{
    internal class VoiceHandler : Script
    {
        [RemoteEvent(Shared.Events.ClientToServer.VoiceChat.SumbitAddVoiceListener)]
        public void ClientEvent_SumbitAddVoiceListener(Client client, ushort remoteID)
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;
            var playa = ClientMethods.FindClientByPlayerID(remoteID);
            if (playa == null) return;
            client.EnableVoiceTo(playa);
            NAPI.Chat.SendChatMessageToAll($"[DEBUG-SERVER]: voice link ACTIVATED: {client.Name} <-> {playa.Name}");
        }
        [RemoteEvent(Shared.Events.ClientToServer.VoiceChat.SumbitRemoveVoiceListener)]
        public void ClientEvent_SumbitRemoveVoiceListener(Client client, ushort remoteID)
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;
            var playa = ClientMethods.FindClientByPlayerID(remoteID);
            if (playa == null) return;
            client.DisableVoiceTo(playa);
            NAPI.Chat.SendChatMessageToAll($"[DEBUG-SERVER]: voice link DE-ACTIVATED: {client.Name} <-> {playa.Name}"); 
        }

        [Command("enablev")]
        public void TEST(Client client, int id)
        {
            var playa = ClientMethods.FindClientByPlayerID(id);
            if (playa == null) return;
            NAPI.Chat.SendChatMessageToAll($"Enabled voice link between {client.Name} <=> {playa.Name}");
            client.EnableVoiceTo(playa);
        }

        [Command("disablev")]
        public void TEST2(Client client, int id)
        {
            var playa = ClientMethods.FindClientByPlayerID(id);
            if (playa == null) return;
            NAPI.Chat.SendChatMessageToAll($"Disabled voice link between {client.Name} <=> {playa.Name}");
            client.DisableVoiceTo(playa);
        }
    }
}
