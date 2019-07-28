using RAGE;
using RAGE.Elements;

namespace RPServerClient.Client
{
    public class Globals : Events.Script
    {
        public Globals()
        {
            Nametags.Enabled = false;
        }

        public static bool IsAccountLoggedIn => (bool)Player.LocalPlayer.GetSharedData(Shared.Data.Keys.AccountLoggedIn);
        public static bool HasActiveChar => (int)Player.LocalPlayer.GetSharedData(Shared.Data.Keys.ActiveCharID) > 0;
        public static int GetActiveCharID => (int)Player.LocalPlayer.GetSharedData(Shared.Data.Keys.ActiveCharID);
    }
}