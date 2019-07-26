using System;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace RPServer.Models
{
    [Table("world")]
    internal class WorldModel : Model<WorldModel>
    {
        public DateTime ServerTime { get; set; }

        public WorldModel() { }

        public static async Task<WorldModel> GetWorldData() => await ReadAsync(0);
        public static async Task SaveWorldData(WorldModel world) => await UpdateAsync(world);
    }
}
