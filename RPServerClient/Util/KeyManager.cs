using System;
using System.Threading.Tasks;
using RAGE;

namespace RPServerClient.Util
{
    public class KeyManager : Events.Script
    {
        private const int ResetTime = 250;
        private static bool _keyStatus = true;

        public static void KeyBind(Shared.Enums.KeyCodes keycode, Action action)
        {
            var key = (int)keycode;
            if (!Input.IsDown(key) || !_keyStatus) return;
            if (!_keyStatus) return;
            action.Invoke();
            _keyStatus = false;
            Task.Delay(ResetTime).ContinueWith(task => _keyStatus = true);
        }

        public static void KeyBind(Shared.Enums.KeyCodes keycode1, Shared.Enums.KeyCodes keycode2, Action action)
        {
            var key1 = (int)keycode1;
            var key2 = (int)keycode2;

            if (!Input.IsDown(key1) || !Input.IsDown(key2) || !_keyStatus) return;

            if (!_keyStatus) return;
            action.Invoke();
            _keyStatus = false;
            Task.Delay(ResetTime).ContinueWith(task => _keyStatus = true);
        }
    }
}
