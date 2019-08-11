using RAGE;
using Shared.Events.ClientToServer;

namespace RPServerClient.Client
{
    internal class EventsHandler : Events.Script
    {
        public EventsHandler()
        {
            Events.OnPlayerCommand += OnPlayerCommandEvent;
        }

        private void OnPlayerCommandEvent(string cmd, Events.CancelEventArgs cancel)
        {
            Events.CallRemote(Command.SubmitPlayerCommand, cmd);
        }
    }
}