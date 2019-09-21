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
        internal bool SpawnItem(SingletonItemTemplate template) => _items.Add(new NonStackableItem(this, template) { Inventory = this });

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

        /// <summary>
        /// Adds an existing item into the inventory. This is transfer of ownership or count adjustment (in the case of stackable items)
        /// </summary>
        /// <param name="item">The specified item</param>
        /// <param name="count">This taken into account only if the Item is a StackableItem</param>
        internal bool TransferItem(NonStackableItem item, uint count = 1)
        {
            var previousInventory = item.Inventory;

            if (previousInventory == this)
            {
                Logger.GetInstance().ServerError("Inventory Error: Source and Destination inventories cannot be the same.");
                return false;
            }

            switch (item)
            {
                // NonStackable
                case NonStackableItem nonStackableItem:
                    switch (nonStackableItem.Template)
                    {
                        case MultitionItemTemplate _:
                            item.Inventory = this;
                            return _items.Add(item);
                        case SingletonItemTemplate singletonItemTemplate:
                            if (HasItem(singletonItemTemplate)) return false;
                            item.Inventory = this;
                            return _items.Add(item);
                    }
                    break;
                case StackableItem stackableItem:
                    var existingItem = GetItemFirstOrNull(stackableItem.Template);
                    if (existingItem == null)
                    { // runs if this inventory does not have an item Instance with this template
                        item.Inventory = this;
                        return _items.Add(item);
                    }
                    // Since we have an existing item with the specified template let's increase it's count
                    var existingStackableItem = existingItem as StackableItem;
                    if (existingStackableItem == null)
                    {
                        Logger.GetInstance().ServerError("Inventory Error: Invalid Cast (2)");
                        return false;
                    }
                    // At this the inventory MUST already have had an item with the same template, so we increase it's count
                    existingStackableItem.Count += stackableItem.Count;
                    break;
            }
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