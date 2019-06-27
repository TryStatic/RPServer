using RAGE;

namespace RPServerClient
{
    class Sandbox : Events.Script
    {
        public Sandbox()
        {
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
    }
}
