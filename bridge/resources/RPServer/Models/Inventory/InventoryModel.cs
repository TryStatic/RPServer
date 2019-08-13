using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    internal class InventoryModel
    {
        private readonly HashSet<ItemModel> _items;

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
                _items.Remove(item);
                await item.Delete();
            }
            else if(item.Amount == instances)
            {
                _items.Remove(item);
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

        public async Task SpawnDroppedItem(int itemID, int dropamount, Vector3 pos, Vector3 rot, uint dimension)
        {
            ItemModel.LastDroppedItemID -= 1;
            var newDropItem = new ItemModel(itemID, ItemModel.LastDroppedItemID, ItemModel.ContainerType.WorldInventory, dropamount) {Dimension = dimension};
            
            var template = ItemTemplate.GetTemplate(itemID);

            var itemPos = new Vector3(pos.X, pos.Y, pos.Z);
            var itemRot = new Vector3(rot.X, rot.Y, rot.Z);

            newDropItem.EntityID = NAPI.Object.CreateObject(template.ObjectID, itemPos, itemRot, 255, newDropItem.Dimension);
            itemPos.Z += 0.2f;
            newDropItem.TextLabel = NAPI.TextLabel.CreateTextLabel($"{template.Name} ({newDropItem.Amount})", itemPos, 5.0f, 0.5f, (int)Shared.Enums.Font.ChaletLondon, new Color(255, 255, 255), false, newDropItem.Dimension);

            WorldHandler.Inventory._items.Add(newDropItem);
            await newDropItem.Create();
        }
    }
}