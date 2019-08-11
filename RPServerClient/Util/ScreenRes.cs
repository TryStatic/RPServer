using System;
using System.Drawing;
using RAGE.Game;

namespace RPServerClient.Util
{
    internal static class ScreenRes
    {
        public const int UIStandardResX = 1280;
        public const int UIStandardResY = 720;

        public static int ClientResX
        {
            get
            {
                var screenX = 0;
                var screenY = 0;
                Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                return screenX;
            }
        }

        public static int ClientResY
        {
            get
            {
                var screenX = 0;
                var screenY = 0;
                Graphics.GetActiveScreenResolution(ref screenX, ref screenY);
                return screenY;
            }
        }

        public static Point ClientResolution
        {
            get
            {
                var pY = 0;
                var pX = 0;
                Graphics.GetActiveScreenResolution(ref pX, ref pY);
                return new Point(pX, pY);
            }
        }

        public static Point ConvertToStandardCoords(Point screenCoords)
        {
            var clientRes = ClientResolution;
            if (screenCoords.X > clientRes.X || screenCoords.Y > clientRes.Y)
            {
                RAGE.Chat.Output("Static fucked up. Report to dev.");
                throw new Exception("ConvertToStandardCoords in ScreenRes.cs");
            }

            var standardX = screenCoords.X / clientRes.X * UIStandardResX;
            var standardY = screenCoords.Y / clientRes.Y * UIStandardResY;

            return new Point(standardX, standardY);
        }
    }
}