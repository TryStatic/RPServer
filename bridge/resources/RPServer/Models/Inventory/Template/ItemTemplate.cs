using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Models.Inventory.Template
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

        public float Weight { protected set; get; } = 0.0f;

        private protected ItemTemplate(string itemName, string itemDesc)
        {
            ID = _lastUsedID++;
            ItemName = itemName;
            ItemDesc = itemDesc;
        }

        #region Fetching
        /// <summary>
        /// Searches for the corresponding ItemTemplate matching the specified template ID.
        /// </summary>
        /// <param name="id">The template ID</param>
        /// <returns>Either the ItemTemplate instance or null if not found</returns>
        public static ItemTemplate GetTemplate(uint id) => ItemTemplates.FirstOrDefault(it => it.ID == id);
        /// <summary>
        /// Retrieves all the available templates as a List
        /// </summary>
        /// <returns>All the templates</returns>
        public static List<ItemTemplate> GetTemplates() => ItemTemplates.ToList();
        #endregion

        #region ItemAction
        protected Action<object[]> ItemAction;
        public bool IsUsable() => ItemAction != null;
        public void InvokeAction(object[] args)
        {
            if(IsUsable()) ItemAction.Invoke(args);
        }
        #endregion

        #region ItemDropInfo
        private static readonly DropObjectInfo DefaultObjectInfo = new DropObjectInfo(NAPI.Util.GetHashKey("hei_prop_heist_box"), new Vector3());
        private DropObjectInfo _dropObjectInfo;
        protected DropObjectInfo DropObjectInfo
        {
            get => _dropObjectInfo ?? DefaultObjectInfo;
            set => _dropObjectInfo = value;
        }
        public DropObjectInfo GetDropObjectInfo() => DropObjectInfo;

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
