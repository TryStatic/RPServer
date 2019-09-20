using System.Linq;
using RPServer.Models.Inventory.Template;

// ReSharper disable SuggestBaseTypeForParameter

namespace RPServer.Models.Inventory.Item
{
    internal abstract class Item
    {
        private static int _lastUnsavedID;

        public int ID { get; set; }
        public Inventory.Inventory Inventory { get; set; }
        public ItemTemplate Template { get; set; }

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