using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPServer.Models.Inventory
{
    internal class InventoryModel
    {
        public HashSet<ItemModel> Items;

        private InventoryModel(HashSet<ItemModel> invItems) => Items = invItems;
        public static async Task<InventoryModel> LoadInventoryAsync(CharacterModel character) => new InventoryModel(await ItemModel.LoadInventoryItems(character));
        public static async Task<InventoryModel> LoadInventoryAsync(VehicleModel vehicle, ItemModel.VehicleContainer container) => new InventoryModel(await ItemModel.LoadInventoryItems(vehicle, container));
        public static async Task<InventoryModel> LoadWorldInventoryAsync() => new InventoryModel(await ItemModel.LoadWorldItems());
    }
}