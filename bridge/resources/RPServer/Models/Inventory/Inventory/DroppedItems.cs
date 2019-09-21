using System.Collections.Generic;
using RPServer.Models.Inventory.Item;

namespace RPServer.Models.Inventory.Inventory
{
    internal class DroppedItems
    {
        private readonly HashSet<DroppedItem> _droppedItems = new HashSet<DroppedItem>();

    }
}