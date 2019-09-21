using GTANetworkAPI;

namespace RPServer.Models.Inventory.Item
{
    internal abstract class DroppedItem
    {
        public Vector3 Position { get; }
        public uint Dimension { get; }

        protected Object Object;
        protected TextLabel TxtLabel;

        protected DroppedItem(Vector3 position, uint dimension)
        {
            Position = position;
            Dimension = dimension;
        }

        internal abstract void Spawn();
        internal abstract void Despawn();
    }
}