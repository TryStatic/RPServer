using System.Drawing;

namespace RPServerClient.Util
{
    internal static class ScreenRes
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

        public static Point ScreenXY
        {
            get
            {
                var pY = 0;
                var pX = 0;
                RAGE.Game.Graphics.GetActiveScreenResolution(ref pX, ref pY);
                return new Point(pX, pY);
            }
        }
    }
}
