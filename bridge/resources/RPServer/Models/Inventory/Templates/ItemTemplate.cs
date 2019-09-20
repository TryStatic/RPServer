using System;
using System.Collections.Generic;
using System.Linq;
using RPServer.Util;

namespace RPServer.Models.Inventory.Templates
{
    /// <summary>
    /// Basic Template to characterize the various items
    /// </summary>
    internal abstract class ItemTemplate
    {
        private static int _lastUsedID;

        public int ID { get; }
        public string ItemName { get; }
        public string ItemDesc { get; }

        private protected ItemTemplate(string itemName, string itemDesc)
        {
            ID = _lastUsedID++;
            ItemName = itemName;
            ItemDesc = itemDesc;
        }

        /// <summary>
        /// Searches for the corresponding ItemTemplate matching the specified template ID.
        /// </summary>
        /// <param name="id">The template ID</param>
        /// <returns>Either the ItemTemplate instance or null if not found</returns>
        public static ItemTemplate GetTemplate(uint id) => ItemTemplates.FirstOrDefault(it => it.ID == id);
        public static List<ItemTemplate> GetTemplates() => ItemTemplates.ToList();

        #region ItemAction
        protected Action<object[]> _itemAction;
        public bool IsUsable() => _itemAction != null;
        public void InvokeAction(object[] args)
        {
            if(IsUsable()) _itemAction.Invoke(args);
        }
        #endregion

        #region LoadingTemplates
        protected static HashSet<ItemTemplate> ItemTemplates = new HashSet<ItemTemplate>();
        protected static bool Loaded;
        internal static void LoadItemTemplates()
        {
            if (Loaded)
            {
                Logger.GetInstance().ServerError("Item Templates were already loaded.");
                return;
            }

            Logger.GetInstance().ServerInfo("Loading Item Templates...");
            StackableItemTemplate.LoadTemplates();
            SingletonItemTemplate.LoadTemplates();
            MultitionItemTemplate.LoadTemplates();
            Logger.GetInstance().ServerInfo($"\tLoaded {ItemTemplates.Count} total item templates.");

            Loaded = true;
        }
        #endregion

        #region HashcodeEquals
        protected bool Equals(ItemTemplate other)
        {
            return ID == other.ID;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ItemTemplate) obj);
        }
        public override int GetHashCode()
        {
            return ID;
        }
        public static bool operator ==(ItemTemplate left, ItemTemplate right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(ItemTemplate left, ItemTemplate right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
