using RAGE.Elements;
using Shared;

namespace RPServerClient.Globals
{
    internal class Globals
    {
        public static bool IsAccountLoggedIn => (bool)Player.LocalPlayer.GetSharedData(Shared.Data.Keys.AccountLoggedIn);

        public static bool HasActiveChar => (int)Player.LocalPlayer.GetSharedData(Shared.Data.Keys.ActiveCharID) > 0;
        public static int GetActiveCharID => (int)Player.LocalPlayer.GetSharedData(Shared.Data.Keys.ActiveCharID);
    }
}
