using System.Collections.Generic;
using System.Drawing;
using RAGE;
using RPServerClient.Util;
using Player = RAGE.Elements.Player;

namespace RPServerClient
{
    internal class Sandbox : Events.Script
    {
        public static int ScreenX = 0;
        public static int ScreenY = 0;

        public static int ScreenResX = 0;
        public static int ScreenResY = 0;

        private static string VERSION = null;

        public Sandbox()
        {
            RAGE.Game.Graphics.GetScreenResolution(ref ScreenResX, ref ScreenResY);
            RAGE.Game.Graphics.GetActiveScreenResolution(ref ScreenX, ref ScreenY);


            Events.Tick += Tick;



            Events.Add("GetVersion", OnGetVersion);


            // FlyScript
            Events.Add("ToggleFlyMode", OnToggleFlyMode);

            // Chars
            Events.Add("headdata", SetHeadData);
            Events.Add("headoverlay", SetHeadOverlay);
            Events.Add("headoverlaycolor", SetHeadOverlayColor);
            Events.Add("headdata", SetHeadData);
            Events.Add("facefeautre", SetFaceFeature);
            Events.Add("compvar", SetCompVar);
            Events.Add("tpinfront", TeleportInFront);
            Events.Add("testpos", TestPos);
            Events.Add("test", Test);

            // Boost
            uint stamina = RAGE.Game.Misc.GetHashKey("SP0_STAMINA");
            RAGE.Game.Stats.StatSetInt(stamina, 100, true);
            uint flying = RAGE.Game.Misc.GetHashKey("SP0_FLYING");
            RAGE.Game.Stats.StatSetInt(flying, 100, true);
            uint driving = RAGE.Game.Misc.GetHashKey("SP0_DRIVING");
            RAGE.Game.Stats.StatSetInt(driving, 100, true);
            uint shooting = RAGE.Game.Misc.GetHashKey("SP0_SHOOTING");
            RAGE.Game.Stats.StatSetInt(shooting, 100, true);
            uint strength = RAGE.Game.Misc.GetHashKey("SP0_STRENGTH");
            RAGE.Game.Stats.StatSetInt(strength, 100, true);
            uint stealth = RAGE.Game.Misc.GetHashKey("SP0_STEALTH");
            RAGE.Game.Stats.StatSetInt(stealth, 100, true);
            uint lungCapacity = RAGE.Game.Misc.GetHashKey("SP0_LUNGCAPACITY");
            RAGE.Game.Stats.StatSetInt(lungCapacity, 100, true);
        }

        private void Test(object[] args)
        {

            RAGE.Chat.Output("Reseting");
            Player.LocalPlayer.SetDefaultComponentVariation();
        }

        private void TestPos(object[] args)
        {
            var vect = Helper.GetPosInFrontOfVector3(new Vector3((float)args[0], (float)args[1], (float)args[2]), (float)args[3], 1);
            RAGE.Chat.Output(vect.ToString());
        }

        private void OnGetVersion(object[] args)
        {
            if (args == null || args.Length < 1) return;
            VERSION = args[0].ToString();
        }


        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (VERSION != null)
            {
                RAGE.Game.Ui.SetTextOutline();
                RAGE.Game.UIText.Draw(VERSION, new Point(ScreenResX / 2, ScreenResY - (int)(ScreenResY * 0.03)), 0.35f, Color.White, RAGE.Game.Font.ChaletLondon, true);
            }

        }

        private void TeleportInFront(object[] args)
        {
            var pos = Helper.GetPosInFrontOfPlayer(Player.LocalPlayer, 2.0f);
            Player.LocalPlayer.Position = pos;
        }

        private void OnToggleFlyMode(object[] args) => Events.CallLocal("flyModeStart");


        private void SetCompVar(object[] args)
        {
            Player.LocalPlayer.SetComponentVariation((int)args[0], (int)args[1], (int)args[2], (int)args[3]);
        }

        private void SetFaceFeature(object[] args)
        {
            Player.LocalPlayer.SetFaceFeature((int)args[0], (float)args[1]);
        }

        private void SetHeadOverlay(object[] args)
        {
            Player.LocalPlayer.SetHeadOverlay((int)args[0], (int)args[1], (float)args[2]);
        }

        private void SetHeadOverlayColor(object[] args)
        {
            RAGE.Chat.Output("Triggert/crted");
            Player.LocalPlayer.SetHeadOverlayColor((int)args[0], (int)args[1], (int)args[2], (int)args[3]);
        }

        private void SetHeadData(object[] args)
        {
            if (args.Length < 9) return;

            Player.LocalPlayer.SetHeadBlendData((int)args[0], (int)args[1], (int)args[2], (int)args[3], (int)args[4], (int)args[5], (float)args[6], (float)args[7], (float)args[8], (bool)args[9]);
        }
    }
}
