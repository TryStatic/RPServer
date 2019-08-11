using System;
using RAGE;
using RPServerClient.Client;
using Shared.Enums;

namespace RPServerClient.Util
{
    public class KeyManager : Events.Script
    {
        private const int ResetTime = 250;
        private static long LatestProcess;

        public static void KeyBind(KeyCodes keycode, Action action)
        {
            if (!Globals.IsAccountLoggedIn || !Globals.HasActiveChar) return;

            var key = (int) keycode;
            if (!Input.IsDown(key)) return;

            var currentTime = (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (currentTime - LatestProcess <= ResetTime) return;
            LatestProcess = currentTime;

            action.Invoke();
        }

        public static void KeyBind(KeyCodes keycode1, KeyCodes keycode2, Action action)
        {
            if (!Globals.IsAccountLoggedIn || !Globals.HasActiveChar) return;

            var key1 = (int) keycode1;
            var key2 = (int) keycode2;
            if (!Input.IsDown(key1) || !Input.IsDown(key2)) return;

            var currentTime = (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (currentTime - LatestProcess <= ResetTime) return;
            LatestProcess = currentTime;

            action.Invoke();
        }
    }
}