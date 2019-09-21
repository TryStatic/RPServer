using System.Collections.Generic;
using System.Linq;
using RPServer.Models.Inventory.Item;
using RPServer.Models.Inventory.Template;
using RPServer.Util;

namespace RPServer.Models.Inventory.Inventory
{
    internal abstract class Inventory
    {
        #region ItemManagement
        private readonly HashSet<Item.Item> _items = new HashSet<Item.Item>();


        /// <summary>
        /// Spawns a new SingletonItem into this inventory.
        /// </summary>
        /// <returns>bool value related to the success or failure of the operation</returns>
        internal bool SpawnItem(SingletonItemTemplate template) => !HasItem(template) && _items.Add(new NonStackableItem(this, template) {Inventory = this});

        /// <summary>
        /// Spawns a new MultitionItem into this inventory.
        /// </summary>
        /// <returns>bool value related to the success or failure of the operation</returns>
        internal bool SpawnItem(MultitionItemTemplate template) => _items.Add(new NonStackableItem(this, template) { Inventory = this });


        /// <summary>
        /// Spawns a new StackableItem into this inventory.
        /// </summary>
        /// <param name="template">The StackableItemTemplate in order to spawn the item</param>
        /// <param name="count">How many "instances" of that item the inventory should hold. MUST BE GREATER THAN 0</param>
        /// <returns>bool value related to the success or failure of the operation</returns>
        internal bool SpawnItem(StackableItemTemplate template, uint count)
        {
            if (count == 0)
            {
                Logger.GetInstance().ServerError("Inventory Error: Cannot Add/spawn StackableItem with count == 0");
                return false;
            }

            var existingItem = GetItemFirstOrNull(template);
            if (existingItem == null) return _items.Add(new StackableItem(this, template, count) { Inventory = this });

            var existingStackableItem = existingItem as StackableItem;
            if (existingStackableItem == null)
            {
                Logger.GetInstance().ServerError("Inventory Error: Invalid Cast (1)");
                return false;
            }

            existingStackableItem.Inventory = this;
            existingStackableItem.Count += count;
            return true;
        }

        internal bool DespawnItem(SingletonItemTemplate template)
        {
            var item = GetItemFirstOrNull(template);
            if (item == null) return false;
            item.Inventory = null;
            return _items.Remove(item);
        }

        internal bool DespawnItem(MultitionItemTemplate template)
        {
            var item = GetItemFirstOrNull(template);
            if (item == null) return false;
            item.Inventory = null;
            return _items.Remove(item);
        }

        internal bool DespawnItem(StackableItemTemplate template, uint count)
        {
            var item = GetItemFirstOrNull(template);
            if (item == null) return false;

            var stackableItem = item as StackableItem;
            if (stackableItem == null)
            {
                Logger.GetInstance().ServerError("Inventory Error: Invalid Cast (3)");
                return false;
            }

            if (stackableItem.Count >= count)
            { // Remove item all-together
                if (stackableItem.Count > count) Logger.GetInstance().ServerError("WARNING: Requested removal of greater count than the inventory has.");
                return _items.Remove(item);
            }

            // Remove from item count
            stackableItem.Count -= count;
            return true;
        }

        /// <summary>
        /// Returns true if there is at least one item with the specified template
        /// </summary>
        internal bool HasItem(ItemTemplate template) => _items.FirstOrDefault(it => it.Template == template) != null;

        /// <summary>
        /// Returns the first occurence with the specified template or null if not found
        /// </summary>
        internal Item.Item GetItemFirstOrNull(ItemTemplate template) => _items.FirstOrDefault(it => it.Template == template);

        /// <summary>
        /// Returns all items that have the specified item template 
        /// </summary>
        internal IEnumerable<Item.Item> GetItems(ItemTemplate template) => _items.Where(it => it.Template == template);
        #endregion
    }
}