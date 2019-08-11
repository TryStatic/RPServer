using System;
using System.Linq;
using GTANetworkAPI;
using RPServer.Controllers.Util;
using RPServer.InternalAPI.Extensions;
using RPServer.Models.Inventory;
using RPServer.Resource;
using static Shared.Data.Colors;

namespace RPServer.Controllers
{
    internal class InventoryHandler : Script
    {
        public InventoryHandler()
        {
            
        }

        [Command(CmdStrings.CMD_Inventory, Alias = CmdStrings.CMD_Inventory_Alias, GreedyArg = true)]
        public void CMD_Inventory(Client client, string args = "")
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;

            var cmdParser = new CommandParser(args);

            if (!cmdParser.HasNextToken())
            {
                InventoryHandler.DisplayInventory(client);
                ChatHandler.SendCommandUsageText(client, CmdStrings.CMD_Inventory_HelpText);
                return;
            }

            switch (cmdParser.GetNextToken())
            {
                case CmdStrings.SUBCMD_Inventory_Use:
                    if (!cmdParser.HasNextToken(typeof(int)))
                    {
                        ChatHandler.SendCommandUsageText(client, "/inv(entory) use [itemID]");
                        return;
                    }
                    var itemUseID = int.Parse(cmdParser.GetNextToken());
                    UseItem(client, itemUseID);
                    break;
                case CmdStrings.SUBCMD_Inventory_Drop:
                    break;
                default:
                    ChatHandler.SendCommandUsageText(client, CmdStrings.CMD_Inventory_HelpText);
                    break;

            }

        }

        private void UseItem(Client client, int itemID)
        {
            var plInvData = client.GetActiveChar().Inventory;
            var item = plInvData.Items.FirstOrDefault(i => i.ItemID == itemID);
            var itemTemplate = ItemTemplate.ItemsTemplate.First(j => j.ID == item.ItemID);
            if (itemTemplate.Action == null)
            {
                ChatHandler.SendCommandErrorText(client, "That item is not usable.");
                return;
            }
            itemTemplate.Action.Invoke(client);
        }

        private static void DisplayInventory(Client client)
        {

            if (client.GetActiveChar().Inventory.Items.Count == 0)
            {
                ChatHandler.SendClientMessage(client, "!{#D4D4D4}Your inventory is empty.");
            }
            else
            {
                ChatHandler.SendClientMessage(client, "!{#D4D4D4}Your owned items:");
                foreach (var i in client.GetActiveChar().Inventory.Items)
                {
                    var item = ItemTemplate.ItemsTemplate.First(t => t.ID == i.ItemID);
                    ChatHandler.SendClientMessage(client, $"\t{COLOR_GRAD3}ID: {i.ItemID}{COLOR_GRAD3} | {COLOR_GRAD3}Item: {item.Name}{COLOR_GRAD3} | Description: {COLOR_WHITE}{item.Desc}{COLOR_GRAD3} | Amount: {COLOR_WHITE}{i.Amount}");
                }
            }
        }
    }
}
