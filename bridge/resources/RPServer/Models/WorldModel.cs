using System;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using RPServer.Controllers;
using RPServer.Models.Inventory;
using RPServer.Util;

namespace RPServer.Models
{
    [Table("world")]
    internal class WorldModel : Model<WorldModel>
    {
        public InventoryModel Inventory;

        public DateTime ServerTime { get; set; }

        public static async Task<WorldModel> GetWorldData()
        {
            return await ReadAsync(0);
        }

        public static async Task SaveWorldData(WorldModel world)
        {
            await UpdateAsync(world);
        }

        public static async Task LoadWorldData()
        {
            Logger.GetInstance().ServerInfo("Loading World Settings.");
            var worldData = await GetWorldData();
            WorldHandler.CurrentTime = worldData.ServerTime;

            Logger.GetInstance().ServerInfo("Loading World Dropped Items.");
            worldData.Inventory = await InventoryModel.LoadWorldInventoryAsync();
        }
    }
}