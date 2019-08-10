using System.Collections.Generic;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    internal class ItemTemplate
    {
        private static List<ItemTemplate> AllItems;

        public int ItemID { set; get; }
        public string Name { set; get; } = "";
        public string Desc { set; get; } = "";

        public static void LoadAllItems()
        {
            if (AllItems != null)
            {
                Logger.GetInstance().ServerError("Item template list is already loaded.");
                return;
            }

            Logger.GetInstance().ServerInfo("Initializing Items Template List.");
            AllItems = new List<ItemTemplate>
            {
                new ItemTemplate()
                {
                    ItemID = 1,
                    Name = "Cash",
                    Desc = "",
                },
                new ItemTemplate()
                {
                    ItemID = 2,
                    Name = "Dice",
                    Desc = "An ordinary dice.",
                }
            };
        }

        #region Generated
        protected bool Equals(ItemTemplate other) => ItemID == other.ItemID;
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ItemTemplate) obj);
        }
        public override int GetHashCode() => ItemID;
        public static bool operator ==(ItemTemplate left, ItemTemplate right) => Equals(left, right);
        public static bool operator !=(ItemTemplate left, ItemTemplate right) => !Equals(left, right);
        #endregion
    }
}
