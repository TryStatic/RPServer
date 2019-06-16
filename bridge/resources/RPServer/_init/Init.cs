using GTANetworkAPI;

namespace RPServer._init
{
    internal class Init : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            // Server Settings
            //NAPI.Server.SetAutoSpawnOnConnect(false);
            //NAPI.Server.SetAutoRespawnAfterDeath(false);

            // Sever World Settings
            NAPI.World.SetTime(0, 0, 0);
            NAPI.World.ResetIplList();


        }
    }
}