using System;
using System.Collections.Generic;
using RAGE;

namespace RPServerClient.Util
{
    internal class Keypress : RAGE.Events.Script
    {
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

            if (Input.IsDown(113) && IsKeypressReady)
            {
                RAGE.Ui.Cursor.Visible = !RAGE.Ui.Cursor.Visible;
                IsKeypressReady = false;
            }
        }
    }
}
