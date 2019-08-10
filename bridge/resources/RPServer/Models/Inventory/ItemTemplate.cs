using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    internal class ItemTemplate
    {
        private static List<ItemTemplate> AllItems;

        public int ItemID { set; get; }
        public string Name { set; get; } = "";
        public string Desc { set; get; } = "";

        public static void LoadAllItems()
        {
            if (AllItems != null)
            {
                Logger.GetInstance().ServerError("Item template list is already loaded.");
                return;
            }

            Logger.GetInstance().ServerInfo("Initializing Items Template List.");
            AllItems = new List<ItemTemplate>
            {
                new ItemTemplate()
                {
                    ItemID = 1,
                    Name = "Cash",
                    Desc = "",
                },
                new ItemTemplate()
                {
                    ItemID = 2,
                    Name = "Dice",
                    Desc = "An ordinary dice.",
                }
            };
        }
    }
}
