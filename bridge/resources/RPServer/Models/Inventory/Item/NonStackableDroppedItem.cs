using GTANetworkAPI;

namespace RPServer.Models.Inventory.Item
{
    internal class NonStackableDroppedItem : DroppedItem
    {
        public NonStackableItem Item { get; }

        public NonStackableDroppedItem(NonStackableItem item, Vector3 position, uint dimension) : base(position, dimension) => Item = item;

        internal override void Spawn()
        {
            var dropInfo = Item.Template.GetDropInfo();
            Object = NAPI.Object.CreateObject(dropInfo.ObjectID, Position, dropInfo.DefaultRotation, 255, Dimension);
            TxtLabel = NAPI.TextLabel.CreateTextLabel($"{Item.Template.ItemName}", Position, 4.0f, 1.0f, 4, new Color(255, 255, 255), false, Dimension);
        }

        internal override void Despawn()
        {
            if (Object != null) Object.Delete();
            if (TxtLabel != null) TxtLabel.Delete();
        }
    }
}
