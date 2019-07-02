using RAGE;
using RAGE.Elements;

namespace RPServerClient
{
    internal class Sandbox : Events.Script
    {
        public Sandbox()
        {
            // FlyScript
            Events.Add("ToggleFlyMode", OnToggleFlyMode);

            // Chars
            Events.Add("headdata", SetHeadData);
            Events.Add("headoverlay", SetHeadOverlay);
            Events.Add("headdata", SetHeadData);
            Events.Add("facefeautre", SetFaceFeature);
            Events.Add("compvar", SetCompVar);

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

        private void SetHeadData(object[] args)
        {
            if (args.Length < 9) return;

            Player.LocalPlayer.SetHeadBlendData((int)args[0], (int)args[1], (int)args[2], (int)args[3], (int)args[4], (int)args[5], (float)args[6], (float)args[7], (float)args[8], (bool)args[9]);
        }
    }
}
