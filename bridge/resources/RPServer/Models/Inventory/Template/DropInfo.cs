using GTANetworkAPI;

namespace RPServer.Models.Inventory.Template
{
    internal class DropInfo
    {
        public DropInfo(uint objectID, Vector3 defaultRotation)
        {
            ObjectID = objectID;
            DefaultRotation = defaultRotation;
        }

        public uint ObjectID { get; }
        public Vector3 DefaultRotation { get; }
    }
}