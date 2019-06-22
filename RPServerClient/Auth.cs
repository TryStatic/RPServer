using RAGE;
using RAGE.Elements;

namespace RPServerClient
{
    internal class Auth : Events.Script
    {
        public Auth()
        {
            Events.Add("SetLoginScreen", OnSetLoginScreen);
        }

        private void OnSetLoginScreen(object[] args)
        {
            var state = (bool)args[0];

            Player.LocalPlayer.FreezePosition(state);

            if (state)
            {
                RAGE.Game.Graphics.TransitionToBlurred(200);

            }
            else
            {
                RAGE.Game.Graphics.TransitionFromBlurred(200);
            }
        }
    }
}
