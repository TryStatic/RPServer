using System.Collections.Generic;
using System.Drawing;
using RAGE;
using RAGE.Game;
using RAGE.Ui;
using RPServerClient.Character.Util;
using RPServerClient.Chat.Util;
using RPServerClient.Client;
using RPServerClient.Util;
using Shared.Enums;
using Font = RAGE.Game.Font;
using Player = RAGE.Elements.Player;

namespace RPServerClient.HUD
{
    internal class MainHud : Events.Script
    {
        public string ServerVersion { get; set; }
        public MainHud()
        {

            Events.Tick += Tick;
            Events.Add(Shared.Events.ServerToClient.HUD.UpdateStaticHudValues, UpdateStaticHUDValues);
        }

        private void UpdateStaticHUDValues(object[] args)
        {
            ServerVersion = (string)Player.LocalPlayer.GetSharedData(Shared.Data.Keys.ServerVersion);
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (ServerVersion != null || !string.IsNullOrWhiteSpace(ServerVersion))
            {
                RAGE.Game.Ui.SetTextOutline();
                RAGE.Game.UIText.Draw(ServerVersion, new Point(ScreenRes.UIStandardResX / 2, ScreenRes.UIStandardResY - (int)(ScreenRes.UIStandardResY * 0.03)), 0.35f, Color.White, RAGE.Game.Font.ChaletLondon, true);
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
