using System;
using RAGE;

namespace RPServerClient.Util
{
    public class KeyManager : Events.Script
    {
        private const int ResetTime = 250;
        private static long LatestProcess = 0;

        public static void KeyBind(Shared.Enums.KeyCodes keycode, Action action)
        {
            var key = (int)keycode;
            if (!Input.IsDown(key)) return;

            long currentTime = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (currentTime - LatestProcess <= ResetTime) return;
            LatestProcess = currentTime;

            action.Invoke();
        }

        public static void KeyBind(Shared.Enums.KeyCodes keycode1, Shared.Enums.KeyCodes keycode2, Action action)
        {
            var key1 = (int)keycode1;
            var key2 = (int)keycode2;
            if (!Input.IsDown(key1) || !Input.IsDown(key2)) return;

            long currentTime = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (currentTime - LatestProcess <= ResetTime) return;
            LatestProcess = currentTime;

            action.Invoke();
        }
    }
}
