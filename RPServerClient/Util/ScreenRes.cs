namespace RPServerClient.Util
{
    internal class ScreenRes : RAGE.Events.Script
    {
        public static int ScreenX
        {
            get
            {
                var screenX = 0;
                var screenY = 0;
                RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                return screenX;
            }
        }

        public static int ScreenY
        {
            get
            {
                var screenX = 0;
                var screenY = 0;
                RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                return screenY;
            }
        }

        public ScreenRes()
        {

        }
    }
}
