using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Models.Inventory.Template
{
    internal class StackableItemTemplate : ItemTemplate
    {
        private StackableItemTemplate(string itemName, string itemDesc) : base(itemName, itemDesc)
        {
        }

        internal static void LoadTemplates()
        {
            if(Loaded) return;
            var count = ItemTemplates.Count;

            ItemTemplates.Add(new StackableItemTemplate("Money", "The standard currency.")
            {
                DropObjectInfo = new DropObjectInfo(NAPI.Util.GetHashKey("bkr_prop_money_sorted_01"), new Vector3())
            });
            ItemTemplates.Add(new StackableItemTemplate("Note", "Some note.")
            {
                Weight = 10.0f
            });

            Logger.GetInstance().ServerInfo($"\t\tLoaded {ItemTemplates.Count - count} Stackable item Templates...");
        }
    }
}