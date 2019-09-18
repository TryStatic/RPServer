using GTANetworkAPI;
using RPServer.InternalAPI;
using RPServer.InternalAPI.Extensions;
using Shared.Events.ClientToServer;

namespace RPServer.Controllers
{
    internal class VoiceHandler : Script
    {
        [RemoteEvent(VoiceChat.SumbitAddVoiceListener)]
        public void ClientEvent_SumbitAddVoiceListener(Client client, ushort remoteID)
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;
            var playa = ClientMethods.FindClientByPlayerID(remoteID);
            if (playa == null) return;
            client.EnableVoiceTo(playa);
        }

        [RemoteEvent(VoiceChat.SumbitRemoveVoiceListener)]
        public void ClientEvent_SumbitRemoveVoiceListener(Client client, ushort remoteID)
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;
            var playa = ClientMethods.FindClientByPlayerID(remoteID);
            if (playa == null) return;
            client.DisableVoiceTo(playa);
        }
    }
}