using System.Linq;
using RPServer.Models.Inventory.Item;
using RPServer.Models.Inventory.Template;

namespace RPServer.Models.Inventory.Inventory
{
    internal enum RefuseReason
    {
        None,
        ExceededMaximumWeight
    }

    internal class PlayerInventory : Inventory
    {
        public float MaxInventoryWeight
        {
            get
            {
                const float baseCharacterWeight = 5000.0f;
                // TODO: Enchance this with backpack items and more
                return baseCharacterWeight;
            }
        }
        public float CurrentWeight
        {
            get
            {
                var totalWeight = 0.0f;
                foreach (var i in _items)
                {
                    switch (i)
                    {
                        case NonStackableItem nonStackableItem:
                            totalWeight += i.Template.Weight;
                            break;
                        case StackableItem stackableItem:
                            totalWeight += i.Template.Weight * stackableItem.Count;
                            break;
                    }
                }
                return totalWeight;
            }
        }

        public bool CanAddItem(ItemTemplate template, uint count = 1) => !(CurrentWeight + template.Weight * count > MaxInventoryWeight);

        public RefuseReason RequestRefuseReason(ItemTemplate template, uint count = 1)
        {
            if (CurrentWeight + template.Weight * count > MaxInventoryWeight) return RefuseReason.ExceededMaximumWeight;
            return RefuseReason.None;
        }

        internal override bool SpawnItem(SingletonItemTemplate template) => CanAddItem(template) && base.SpawnItem(template);
        internal override bool SpawnItem(MultitionItemTemplate template) => CanAddItem(template) && base.SpawnItem(template);
        internal override bool SpawnItem(StackableItemTemplate template, uint count) => CanAddItem(template) && base.SpawnItem(template, count);
        internal override bool DespawnItem(SingletonItemTemplate template) => CanAddItem(template) && base.SpawnItem(template);
        internal override bool DespawnItem(MultitionItemTemplate template) => CanAddItem(template) && base.SpawnItem(template);
        internal override bool DespawnItem(StackableItemTemplate template, uint count) => CanAddItem(template) && base.SpawnItem(template, count);
    }
}