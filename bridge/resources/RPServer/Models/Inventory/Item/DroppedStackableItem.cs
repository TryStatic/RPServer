using GTANetworkAPI;

namespace RPServer.Models.Inventory.Item
{
    internal class DroppedStackableItem : DroppedItem
    {
        public StackableItem Item { get; }
        public uint Count { get; }

        public DroppedStackableItem(StackableItem item, uint count, Vector3 position, uint dimension) : base(position, dimension)
        {
            if (count == 0) count = 1;

            Item = item;
            Count = count;
        }

        internal override void Spawn()
        {
            var dropObjectInfo = Item.Template.GetDropObjectInfo();
            if (dropObjectInfo != null)
            {
                Object = NAPI.Object.CreateObject(dropObjectInfo.ObjectID, Position, dropObjectInfo.DefaultRotation, 255, Dimension);
            }

            TxtLabel = NAPI.TextLabel.CreateTextLabel($"{Item.Template.ItemName} ({Count})", Position, 4.0f, 1.0f, 4, new Color(255, 255, 255), false, Dimension);
        }

        internal override void Despawn()
        {
            if (Object != null) Object.Delete();
            if (TxtLabel != null) TxtLabel.Delete();
        }
    }
}