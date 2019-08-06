using System.Collections.Generic;
using System.Drawing;
using RAGE;
using RAGE.Game;
using RPServerClient.Chat.Util;
using RPServerClient.Client;
using RPServerClient.Util;
using Font = RAGE.Game.Font;
using Player = RAGE.Elements.Player;

namespace RPServerClient.HUD
{
    internal class MainHud : Events.Script
    {
        public MainHud()
        {

            Events.Tick += Tick;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (Globals.ServerVersion != null)
            {
                RAGE.Game.Ui.SetTextOutline();
                RAGE.Game.UIText.Draw(Globals.ServerVersion, new Point(ScreenRes.UIStandardResX / 2, ScreenRes.UIStandardResY - (int)(ScreenRes.UIStandardResY * 0.03)), 0.35f, Color.White, RAGE.Game.Font.ChaletLondon, true);
            }

            if (Globals.IsAccountLoggedIn && Globals.HasActiveChar)
            {
                var mode = Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);
                RAGE.Game.Ui.SetTextOutline();
                UIText.Draw(mode.ToString(), new Point((int) (0.03f * ScreenRes.UIStandardResX), (int)(0.26f * ScreenRes.UIStandardResY)), 0.4f, Color.White, Font.Monospace, true);
            }
        }
    }
}
