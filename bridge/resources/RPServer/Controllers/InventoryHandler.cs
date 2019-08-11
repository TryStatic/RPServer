using GTANetworkAPI;
using RPServer.Controllers.Util;
using RPServer.InternalAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Models.Inventory;
using RPServer.Resource;

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
                ChatHandler.SendCommandUsageText(client, CmdStrings.CMD_Inventory_HelpText);
                return;
            }

            switch (cmdParser.GetNextToken())
            {
                case "list": // list
                    DisplayInventoryItems(client, client.GetActiveChar().Inventory);
                    break;
                case CmdStrings.SUBCMD_Inventory_Use:
                    if (!cmdParser.HasNextToken(typeof(int)))
                    {
                        ChatHandler.SendCommandUsageText(client, "/inv(entory) use [itemID]");
                        return;
                    }

                    var useToken = cmdParser.GetNextToken();
                    var useItemID = int.Parse(useToken);

                    if (!ItemTemplate.IsValidItemTemplateID(useItemID))
                    {
                        ChatHandler.SendCommandErrorText(client, "This is not a valid itemID.");
                        return;
                    }

                    var template = ItemTemplate.GetTemplate(useItemID);
                    if (template.SelfAction == null)
                    {
                        ChatHandler.SendCommandErrorText(client, "This is not a usable item.");
                        return;
                    }

                    template.SelfAction.Invoke(client);

                    break;
                case "give": // give
                    if (!cmdParser.HasNextToken())
                    {
                        ChatHandler.SendCommandUsageText(client, "/inv(entory) give [PartOfName/PlayerID] [ItemID]");
                        return;
                    }

                    var otherClient = ClientMethods.FindClient(cmdParser.GetNextToken());
                    if (otherClient == null)
                    {
                        ChatHandler.SendCommandErrorText(client, "We couldn't find that player.");
                        return;
                    }

                    if (!client.IsLoggedIn() || !client.HasActiveChar())
                    {
                        ChatHandler.SendCommandErrorText(client, "That player is not logged in.");
                        return;
                    }

                    if (!cmdParser.HasNextToken(typeof(int)))
                    {
                        ChatHandler.SendCommandUsageText(client, "/inv(entory) give PartOfName/PlayerID [ItemID]");
                        return;
                    }
                    var giveToken = cmdParser.GetNextToken();
                    var giveItemID = int.Parse(giveToken);

                    if (!ItemTemplate.IsValidItemTemplateID(giveItemID))
                    {
                        ChatHandler.SendCommandErrorText(client, "This is not a valid itemID.");
                        return;
                    }

                    var giveTemplate = ItemTemplate.GetTemplate(giveItemID);

                    if (!giveTemplate.Tradeable)
                    {
                        ChatHandler.SendCommandErrorText(client, "That item cannot be traded.");
                        return;
                    }

                    var playerInv = client.GetActiveChar().Inventory;
                    var otherInv = client.GetActiveChar().Inventory;



                    break;
                case CmdStrings.SUBCMD_Inventory_Drop: // drop
                    break;
                case "destroy": // destroy
                    break;
                default:
                    ChatHandler.SendCommandUsageText(client, CmdStrings.CMD_Inventory_HelpText);
                    break;

            }

        }


        /// <summary>
        /// </summary>
        /// <param name="client">The Client to display the items</param>
        /// <param name="inventory">The actual inventory.</param>
        private static void DisplayInventoryItems(Client client, InventoryModel inventory)
        {
            foreach (var item in inventory.Items)
            {
                var itemInfo = ItemTemplate.GetTemplate(item.ItemID);
                ChatHandler.SendClientMessage(client, $"ItemID: {item.ItemID} => {itemInfo.Name}, {itemInfo.Desc}, {itemInfo.GetItemType().ToString()}, {item.Amount}");
            }
            ChatHandler.SendClientMessage(client, "-----------------------------------");
        }
    }
}
