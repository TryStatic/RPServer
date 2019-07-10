using RAGE.Elements;
using Shared;

namespace RPServerClient.Globals
{
    internal class Globals : RAGE.Events.Script
    {
        public static bool IsAccountLoggedIn => (bool)Player.LocalPlayer.GetSharedData(SharedDataKey.AccountLoggedIn);

        public static bool HasActiveChar => (int)Player.LocalPlayer.GetSharedData(SharedDataKey.ActiveCharID) > 0;
        public static int GetActiveCharID => (int)Player.LocalPlayer.GetSharedData(SharedDataKey.ActiveCharID);
    }
}
