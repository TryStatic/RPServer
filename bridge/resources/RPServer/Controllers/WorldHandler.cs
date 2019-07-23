using System;
using System.Threading;
using GTANetworkAPI;

namespace RPServer.Controllers
{
    internal class WorldHandler : Script
    {
        public static DateTime CurrentTime { set; get; }

        private static Timer _updateTimeTimer;

        public WorldHandler()
        {
            AppDomain.CurrentDomain.ProcessExit += OnServerShutdown;
            _updateTimeTimer = new Timer(OnUpdateTime, null, 0, 1000);
        }
        
        public bool IsDayTime() => CurrentTime.Hour > 6 || CurrentTime.Hour < 21;

        private void OnUpdateTime(object state)
        {
            CurrentTime = CurrentTime.AddSeconds(4.0);
            NAPI.World.SetTime(CurrentTime.Hour, CurrentTime.Minute, CurrentTime.Second);
        }

        private async void OnServerShutdown(object sender, EventArgs e)
        {
            // Save World Data
            var worldData = await Models.World.GetWorldData();
            worldData.ServerTime = CurrentTime;
            await Models.World.SaveWorldData(worldData);

            // Dispose Timers
            _updateTimeTimer.Dispose();
        }
    }
}
