using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Models.Inventory.Template
{
    internal class SingletonItemTemplate : ItemTemplate
    {
        private SingletonItemTemplate(string itemName, string itemDesc) : base(itemName, itemDesc)
        {
        }

        internal static void LoadTemplates()
        {
            if (Loaded) return;
            var count = ItemTemplates.Count;

            ItemTemplates.Add(new SingletonItemTemplate("Teddy Bear", "Just a Teddy Bear")
            {
                Weight = 50.0f
            });
            ItemTemplates.Add(new SingletonItemTemplate("Water Bottle", "A water bottle")
            {
                Weight = 500.0f,
                DropObjectInfo = new DropObjectInfo(NAPI.Util.GetHashKey("ng_proc_beerbottle_01a"), new Vector3())
            });

            Logger.GetInstance().ServerInfo($"\t\tLoaded {ItemTemplates.Count - count} Singleton item Templates...");
        }
    }
}