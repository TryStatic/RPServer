using GTANetworkAPI;

namespace RPServer.Models.Inventory.Item
{
    internal class DroppedNonStackableItem : DroppedItem
    {
        public NonStackableItem Item { get; }

        public DroppedNonStackableItem(NonStackableItem item, Vector3 position, uint dimension) : base(position, dimension) => Item = item;

        internal override void Spawn()
        {
            // Object Spawning
            var dropObjectInfo = Item.Template.GetDropObjectInfo();
            if (dropObjectInfo != null)
            {
                Object = NAPI.Object.CreateObject(dropObjectInfo.ObjectID, Position, dropObjectInfo.DefaultRotation, 255, Dimension);
            }

            // Label Spawning
            TxtLabel = NAPI.TextLabel.CreateTextLabel($"{Item.Template.ItemName}", Position, 4.0f, 1.0f, 4, new Color(255, 255, 255), false, Dimension);
        }

        internal override void Despawn()
        {
            if (Object != null) Object.Delete();
            if (TxtLabel != null) TxtLabel.Delete();
        }
    }
}
