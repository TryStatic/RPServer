using System;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace RPServer.Models
{
    [Table("world")]
    internal class World : Model<World>
    {
        public DateTime ServerTime { get; set; }

        public World() { }

        public static async Task<World> GetWorldData() => await ReadAsync(0);
        public static async Task SaveWorldData(World world) => await UpdateAsync(world);
    }
}
