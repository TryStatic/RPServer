using System.Collections.Generic;
using System.Drawing;
using RAGE;
using RAGE.NUI;
using RAGE.Ui;
using RPServerClient.Character.Util;
using RPServerClient.Client;
using RPServerClient.Util;
using Shared.Enums;
using Font = RAGE.Game.Font;
using Player = RAGE.Elements.Player;

namespace RPServerClient.HUD
{
    class MainHUD : Events.Script
    {
        public static int ScreenX;
        public static int ScreenY;

        public static int ScreenResX;
        public static int ScreenResY;

        public string ServerVersion { get; set; }
        public MainHUD()
        {
            RAGE.Game.Graphics.GetScreenResolution(ref ScreenResX, ref ScreenResY);
            RAGE.Game.Graphics.GetActiveScreenResolution(ref ScreenX, ref ScreenY);


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
                RAGE.Game.UIText.Draw(ServerVersion, new Point(ScreenResX / 2, ScreenResY - (int)(ScreenResY * 0.03)), 0.35f, Color.White, RAGE.Game.Font.ChaletLondon, true);
            }

            if (Globals.IsAccountLoggedIn && Globals.HasActiveChar)
            {
                var mode = Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);
                RAGE.NUI.UIResText.Draw(mode.ToString(), new Point(24, 232), 0.4f, Color.White, Font.Monospace, true);
            }
            KeyManager.KeyBind(KeyCodes.VK_G, () =>
            {
                RAGE.Chat.Output((Cursor.Position.X / ScreenX) * 1280 + " " + (Cursor.Position.Y / ScreenY) * 720);
            });
        }
    }
}
