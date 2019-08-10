using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    internal class ItemTemplate
    {
        public static List<ItemTemplate> AllItems;

        public string Name { set; get; }
        public string Desc { set; get; }

        public static void LoadAllItems()
        {
            if (AllItems != null)
            {
                Logger.GetInstance().ServerError("Item templates are already loaded.");
                return;
            }

            AllItems = new List<ItemTemplate>
            {
                new ItemTemplate()
                {
                    Name = "Dice",
                    Desc = "An ordinary dice.",
                },
                new ItemTemplate()
                {
                    Name = "Test",
                    Desc = "A Test Item",
                }
            };
        }
    }
}
