using System;
using System.Collections.Generic;
using RAGE;

namespace RPServerClient.Util
{
    internal class Keypress : RAGE.Events.Script
    { // Todo: Add a proper key handler
        public static bool IsKeypressReady = true;
        public static double ticks = 0;
        public static double around_one_sec = 80;

        public Keypress()
        {
            Events.Tick += OnRender;
        }

        private void OnRender(List<Events.TickNametagData> nametags)
        {
            ticks++;

            if (Math.Abs(ticks - around_one_sec) < 0.01)
            {
                ticks = 0;
                IsKeypressReady = true;
            }

            if (Input.IsDown((int)Shared.Enums.KeyCodes.VK_F2) && IsKeypressReady)
            { // TODO: Move me out of here and after a proper key handler is added
                RAGE.Ui.Cursor.Visible = !RAGE.Ui.Cursor.Visible;
                IsKeypressReady = false;
            }
        }
    }
}
