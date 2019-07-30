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
            if (client.IsLoggedIn() || client.HasActiveChar()) return;
            var playa = ClientMethods.FindClientByPlayerID(remoteID);
            if (playa == null) return;
            client.EnableVoiceTo(playa);
        }
        [RemoteEvent(Shared.Events.ClientToServer.VoiceChat.SumbitRemoveVoiceListener)]
        public void ClientEvent_SumbitRemoveVoiceListener(Client client, ushort remoteID)
        {
            if (client.IsLoggedIn() || client.HasActiveChar()) return;
            var playa = ClientMethods.FindClientByPlayerID(remoteID);
            if (playa == null) return;
            client.DisableVoiceTo(playa);
        }
    }
}
