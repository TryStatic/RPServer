using RPServer.Util;

namespace RPServer.Models.Inventory.Templates
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

            ItemTemplates.Add(new SingletonItemTemplate("Teddy Bear", "Just a Teddy Bear"));
            ItemTemplates.Add(new SingletonItemTemplate("Water Bottle", "A water bottle"));

            Logger.GetInstance().ServerInfo($"\t\tLoaded {ItemTemplates.Count - count} Singleton item Templates...");
        }
    }
}