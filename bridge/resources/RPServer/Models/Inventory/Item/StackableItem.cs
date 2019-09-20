using RPServer.Models.Inventory.Template;

namespace RPServer.Models.Inventory.Item
{
    internal class StackableItem : Item
    {
        public uint Count { get; set; }

        public StackableItem(Inventory.Inventory inventory, StackableItemTemplate template, uint count) : base(inventory, template)
        {
            if (count == 0) count = 1;
            Count = count;
        }

        public StackableItem(int id, Inventory.Inventory inventory, StackableItemTemplate template, uint count) : base(id, inventory, template)
        {
            if (count == 0) count = 1;
            Count = count;
        }
    }
}