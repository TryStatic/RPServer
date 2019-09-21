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

        internal void AddItem(SingletonItemTemplate template)
        {
            if (HasItem(template)) return;
            _items.Add(new NonStackableItem(this, template));
        }

        internal void AddItem(MultitionItemTemplate template)
        {
            _items.Add(new NonStackableItem(this, template));
        }

        internal void AddItem(StackableItemTemplate template, uint count)
        {
            if(count == 0) return;

            var existingItem = GetItemFirstOrNull(template);
            if (existingItem == null)
            {
                _items.Add(new StackableItem(this, template, count));
                return;
            }

            var existingStackableItem = existingItem as StackableItem;
            if (existingStackableItem == null)
            {
                Logger.GetInstance().ServerError("Inventory Error: Invalid Cast (1)");
                return;
            }
            existingStackableItem.Count += count;
        }

        internal void AddItem(Item.Item item)
        {
            if (item.Inventory == this || item.Inventory == null)
            {
                Logger.GetInstance().ServerError("Inventory Error: Found item with null inventory refrence");
                return;
            }

            switch (item)
            {
                case NonStackableItem nonStackableItem:
                    switch (nonStackableItem.Template)
                    {
                        case MultitionItemTemplate multitionItemTemplate:
                            item.Inventory = this;
                            _items.Add(item);
                            break;
                        case SingletonItemTemplate singletonItemTemplate:
                            if (HasItem(singletonItemTemplate)) return;
                            item.Inventory = this;
                            _items.Add(item);
                            break;
                    }
                    break;
                case StackableItem stackableItem:
                    var existingItem = GetItemFirstOrNull(stackableItem.Template);
                    if (existingItem == null)
                    {
                        item.Inventory = this;
                        _items.Add(item);
                        return;
                    }
                    var existingStackableItem = existingItem as StackableItem;
                    if (existingStackableItem == null)
                    {
                        Logger.GetInstance().ServerError("Inventory Error: Invalid Cast (2)");
                        return;
                    }
                    existingStackableItem.Count += stackableItem.Count;
                    stackableItem.Inventory = null;
                    break;
            }
        }



        internal void RemoveItem(SingletonItemTemplate template)
        {
            var item = GetItemFirstOrNull(template);
            if (item != null) _items.Remove(item);
        }

        internal void RemoveItem(MultitionItemTemplate template)
        {
            var item = GetItemFirstOrNull(template);
            if (item != null) _items.Remove(item);
        }

        internal void RemoveItem(StackableItemTemplate template, uint count)
        {
            var item = GetItemFirstOrNull(template);
            if (item == null) return;
            var stackableItem = item as StackableItem;
            if (stackableItem == null)
            {
                Logger.GetInstance().ServerError("Inventory Error: Invalid Cast (3)");
                return;
            }

            if (stackableItem.Count >= count)
            { // Remove item all-together
                if (stackableItem.Count > count) Logger.GetInstance().ServerError("WARNING: Requested removal of greater count than the inventory has");
                _items.Remove(item);
            }
            else
            { // Remove from item count
                stackableItem.Count -= count;
            }
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
        internal IEnumerable<Item.Item> GetItem(ItemTemplate template) => _items.Where(it => it.Template == template);
        #endregion
    }
}