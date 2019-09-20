using RPServer.Models.Inventory.Template;

namespace RPServer.Models.Inventory.Item
{
    internal class NonStackableItem : Item
    {
        public NonStackableItem(Inventory inventory, MultitionItemTemplate template) : base(inventory, template)
        {
        }

        public NonStackableItem(int id, Inventory inventory, MultitionItemTemplate template) : base(id, inventory, template)
        {
        }

        public NonStackableItem(Inventory inventory, SingletonItemTemplate template) : base(inventory, template)
        {
        }

        public NonStackableItem(int id, Inventory inventory, SingletonItemTemplate template) : base(id, inventory, template)
        {
        }
    }
}