using RPServer.Util;

namespace RPServer.Models.Inventory.Templates
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

            ItemTemplates.Add(new StackableItemTemplate("Money", "The standard currency."));
            ItemTemplates.Add(new StackableItemTemplate("Note", "Some note."));

            Logger.GetInstance().ServerInfo($"\t\tLoaded {ItemTemplates.Count - count} Stackable item Templates...");
        }
    }
}