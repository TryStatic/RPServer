namespace RPServerClient.Globals
{
    internal class Events : RAGE.Events.Script
    {
        public Events()
        {
            RAGE.Events.OnPlayerCommand += OnPlayerCommandEvent;
        }

        private void OnPlayerCommandEvent(string cmd, RAGE.Events.CancelEventArgs cancel)
        {
            RAGE.Events.CallRemote(Shared.Events.ClientToServer.Command.SubmitPlayerCommand, cmd);
        }
    }
}
