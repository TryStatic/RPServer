using Shared;
using RAGE.Elements;

namespace RPServerClient.Globals
{
    internal class Globals : RAGE.Events.Script
    {
        public static bool IsAccountLoggedIn => (bool) Player.LocalPlayer.GetSharedData(SharedDataKey.AccountLoggedIn);
    }
}
