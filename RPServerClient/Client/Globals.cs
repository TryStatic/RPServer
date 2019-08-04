using RAGE;
using RAGE.Elements;
using Shared.Data;

namespace RPServerClient.Client
{
    public class Globals : Events.Script
    {
        public Globals()
        {
            Nametags.Enabled = false;
        }

        public static bool IsAccountLoggedIn => Player.LocalPlayer.GetSharedData(Keys.AccountLoggedIn) != null && (bool) Player.LocalPlayer.GetSharedData(Keys.AccountLoggedIn);
        public static bool HasActiveChar => Player.LocalPlayer.GetSharedData(Keys.ActiveCharID) != null && (int) Player.LocalPlayer.GetSharedData(Keys.ActiveCharID) > 0;
        public static int GetActiveCharID => Player.LocalPlayer.GetSharedData(Keys.ActiveCharID) == null ? -1 : (int) Player.LocalPlayer.GetSharedData(Keys.ActiveCharID);

    }
}