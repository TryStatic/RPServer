namespace RPServerClient.Client
{
    internal class EventsHandler : RAGE.Events.Script
    {
        public EventsHandler()
        {
            RAGE.Events.OnPlayerCommand += OnPlayerCommandEvent;
        }

        private void OnPlayerCommandEvent(string cmd, RAGE.Events.CancelEventArgs cancel)
        {
            RAGE.Events.CallRemote(Shared.Events.ClientToServer.Command.SubmitPlayerCommand, cmd);
        }
    }
}
