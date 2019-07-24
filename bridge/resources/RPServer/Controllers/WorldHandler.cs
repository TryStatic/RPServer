using System;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Controllers
{
    internal class WorldHandler : Script
    {
        public static DateTime CurrentTime { set; get; }

        private static Timer _updateTimeTimer;

        public WorldHandler()
        {
            _updateTimeTimer = new Timer(OnUpdateTime, null, 0, 1000);
        }
        
        public bool IsDayTime() => CurrentTime.Hour > 6 || CurrentTime.Hour < 21;

        private void OnUpdateTime(object state)
        {
            CurrentTime = CurrentTime.AddSeconds(4.0);
            NAPI.World.SetTime(CurrentTime.Hour, CurrentTime.Minute, CurrentTime.Second);
        }

        public static async Task OnServerShutdown()
        { 
            Logger.GetInstance().ServerInfo("[SHUTDOWN]: Started saving World Data.");
            // Save World Data
            var worldData = await Models.World.GetWorldData();
            worldData.ServerTime = CurrentTime;
            await Models.World.SaveWorldData(worldData);
            Logger.GetInstance().ServerInfo("[SHUTDOWN]: Finished saving World Data.");
            // Dispose Timers
            _updateTimeTimer.Dispose();
        }
    }
}
