using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    internal class InventoryModel
    {
        private HashSet<ItemModel> _items;

        private InventoryModel(HashSet<ItemModel> invItems)
        {
            _items = invItems;
        }

        public static async Task<InventoryModel> LoadInventoryAsync(CharacterModel character)
        {
            return new InventoryModel(await ItemModel.LoadInventoryItems(character));
        }

        public static async Task<InventoryModel> LoadInventoryAsync(VehicleModel vehicle,
            ItemModel.VehicleContainer container)
        {
            return new InventoryModel(await ItemModel.LoadInventoryItems(vehicle, container));
        }

        public static async Task<InventoryModel> LoadWorldInventoryAsync()
        {
            return new InventoryModel(await ItemModel.LoadWorldItems());
        }
        

        public bool HasItem(int itemID) => _items.FirstOrDefault(it => it.ItemID == itemID) != null;
        public bool HasItem(int itemID, int amount)
        {
            var item = _items.FirstOrDefault(it => it.ItemID == itemID);
            if (item == null) return false;
            return item.Amount >= amount;
        }

        public HashSet<ItemModel> GetItems() => new HashSet<ItemModel>(_items);

        public async Task SpawnItem(CharacterModel character, int itemID, int amount = 1)
        {
            var item = _items.FirstOrDefault(it => it.ItemID == itemID);
            if (item != null)
            {
                item.Amount += amount;
                await item.Update();
            }
            else
            {
                var newItem = new ItemModel(itemID, character.ID, ItemModel.ContainerType.CharacterInventory, amount);
                _items.Add(newItem);
                await newItem.Create();
            }
        }

        public async Task DestroyItem(int itemID, int instances = 1)
        {
            var item = _items.FirstOrDefault(it => it.ItemID == itemID);
            if (item == null)
            {
                Logger.GetInstance().ServerError("Tried to delete inexistant item from inventory.");
                return;
            }

            if (item.Amount < instances) {
                Logger.GetInstance().ServerError("Tried to delete more instances of a specific item than it currently has, deleting item completely.");
                await item.Delete();
            }
            else if(item.Amount == instances)
            {
                await item.Delete();
            }
            else
            {
                item.Amount -= instances;
                await item.Update();
            }
        }

        public bool CanAddItem(int giveItemID, int amount)
        {
            return true;
        }
    }
}