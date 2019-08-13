using System;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;
using RPServer.Models;
using RPServer.Models.Inventory;
using RPServer.Util;

namespace RPServer.Controllers
{
    internal class WorldHandler : Script
    {
        public static InventoryModel Inventory;

        private static Timer _updateTimeTimer;
        private static Timer _saveWorldDataTimer;

        public WorldHandler()
        {
            _updateTimeTimer = new Timer(OnUpdateTime, null, 0, 1000);
            _saveWorldDataTimer = new Timer(OnSaveWorldData, null, 5000 * 60 * 5, 5000 * 60 * 5); // 5 minutes
        }

        public static DateTime CurrentTime { set; get; }

        public bool IsDayTime()
        {
            return CurrentTime.Hour > 6 || CurrentTime.Hour < 21;
        }

        public static async Task OnServerShutdown()
        {
            Logger.GetInstance().ServerInfo("[SHUTDOWN]: Started saving World Data.");
            // Save World Data
            await SaveWorldData();
            Logger.GetInstance().ServerInfo("[SHUTDOWN]: Finished saving World Data.");
            // Dispose Timers
            _updateTimeTimer.Dispose();
            _saveWorldDataTimer.Dispose();
        }

        private static void OnUpdateTime(object state)
        {
            CurrentTime = CurrentTime.AddSeconds(4.0);
            NAPI.World.SetTime(CurrentTime.Hour, CurrentTime.Minute, CurrentTime.Second);
        }

        private static async void OnSaveWorldData(object state)
        {
            await SaveWorldData();
        }

        private static async Task SaveWorldData()
        {
            var worldData = await WorldModel.GetWorldData();
            if (worldData == null) return;
            // Add world data here
            worldData.ServerTime = CurrentTime;
            await WorldModel.SaveWorldData(worldData);
        }
    }
}