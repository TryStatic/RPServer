using System;
using System.Linq;
using RPServer.Models.Inventory.Template;
using RPServer.Util;

// ReSharper disable SuggestBaseTypeForParameter

namespace RPServer.Models.Inventory.Item
{
    internal abstract class Item
    {
        private static int _lastUnsavedID;

        public int ID { get; }
        public Inventory.Inventory Inventory { get; set; }
        public ItemTemplate Template { get; }

        /// <summary>
        /// Use for spawning entirely new Stackable items
        /// </summary>
        private protected Item(Inventory.Inventory inventory, ItemTemplate template)
        {
            ID = --_lastUnsavedID;
            Inventory = inventory;
            Template = template;
        }

        /// <summary>
        /// Use when loading already saved Stackable items from the database
        /// </summary>
        private protected Item(int id, Inventory.Inventory inventory, ItemTemplate template)
        {
            ID = id;
            Inventory = inventory;
            Template = template;
        }

        /// <summary>
        /// Item transfer between inventories.
        /// </summary>
        /// <param name="destinationInventory"></param>
        /// <param name="item">The specified item</param>
        /// <param name="count">This taken into account only if the Item is a StackableItem</param>
        internal bool TransferItem(Inventory.Inventory destinationInventory, uint count = 1)
        {
            var sourceInventory = Inventory;

            if (sourceInventory == null)
            {
                Logger.GetInstance().ServerError("This is a dropped item, it cannot be trasfered via TransferItem.");
                return false;
            }

            if (destinationInventory == null)
            {
                Logger.GetInstance().ServerError("Destination inventory cannot be null.");
                return false;
            }

            if (sourceInventory == destinationInventory)
            {
                Logger.GetInstance().ServerError("Inventory Error: Source and Destination inventories cannot be the same.");
                return false;
            }

            switch (this)
            {
                // NonStackable
                case NonStackableItem nonStackableItem:
                {
                    switch (nonStackableItem.Template)
                    {
                        case MultitionItemTemplate multitionItemTemplate:
                        {
                            if (destinationInventory.SpawnItem(multitionItemTemplate))
                            { // If added successfully to destination inventory

                                if (sourceInventory.DespawnItem(multitionItemTemplate))
                                { // Despawned successfully from source inventory
                                    return true;
                                }

                                // Couldn't not despawn it from source so despawn it from destination
                                if (destinationInventory.DespawnItem(multitionItemTemplate))
                                {
                                    return false;
                                }

                                Logger.GetInstance().ServerError("Couldn't despawn item possible duplicate.");
                                return false;
                            }
                            
                            // Couldn't add it to destination for some reason
                            Logger.GetInstance().ServerError("Could not add item");
                            return false;
                        }
                        case SingletonItemTemplate singletonItemTemplate:
                        {
                            if (destinationInventory.HasItem(singletonItemTemplate)) return false;

                            if (destinationInventory.SpawnItem(singletonItemTemplate))
                            { // If added successfully to destination inventory

                                if (sourceInventory.DespawnItem(singletonItemTemplate))
                                { // Despawned successfully from source inventory
                                    return true;
                                }

                                // Couldn't not despawn it from source so despawn it from destination
                                if (destinationInventory.DespawnItem(singletonItemTemplate))
                                {
                                    return false;
                                }

                                Logger.GetInstance().ServerError("Couldn't despawn item possible duplicate.");
                                return false;
                            }

                            // Couldn't add it to destination for some reason
                            Logger.GetInstance().ServerError("Could not add item");
                            return false;
                        }
                    }
                    break;
                }
                case StackableItem stackableItem:
                    {
                        // If source inventory doesn't have that many of that item
                        if (count > stackableItem.Count) return false;

                        if (destinationInventory.HasItem(Template))
                        { // if destination inventory has at least one of the same stackableItem
                            var destItem = destinationInventory.GetItemFirstOrNull(Template) as StackableItem;
                            if (destItem == null)
                            {
                                Logger.GetInstance().ServerError("Inventory cast error (10)");
                                return false;
                            }

                            // Increase it's count
                            destItem.Count += count;

                            // Try to remove it from our inventory
                            if (stackableItem.Count == count)
                            { // If you they are transferring all counts of that item
                                if (sourceInventory.DespawnItem((StackableItemTemplate)Template, count)) return true;

                                // Rollback
                                Logger.GetInstance().ServerError("Couldn't not remove item from source inventory. (1)");
                                destItem.Count -= count;
                                return false;
                            }

                            // We know it's not greater (checked at the beginning) and we know it's not equal so it's less.
                            stackableItem.Count -= count;
                            return true;
                        }
                        break;
                    }
            }

            return false;
        }

        #region HashCodeEquals
        protected bool Equals(Item other)
        {
            return ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Item) obj);
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public static bool operator ==(Item left, Item right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Item left, Item right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}