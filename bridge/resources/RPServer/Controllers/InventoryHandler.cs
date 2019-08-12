using System.Linq;
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
        [Command(CmdStrings.CMD_Inventory, Alias = CmdStrings.CMD_Inventory_Alias, GreedyArg = true)]
        public async void CMD_Inventory(Client client, string args = "")
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

                    var plInvent = client.GetActiveChar().Inventory;

                    if (!plInvent.HasItem(useItemID))
                    {
                        ChatHandler.SendCommandErrorText(client, "You don't own that item.");
                        return;
                    }

                    var template = ItemTemplate.GetTemplate(useItemID);
                    if (template.SelfAction == null)
                    {
                        ChatHandler.SendCommandErrorText(client, "This is not a usable item.");
                        return;
                    }

                    template.SelfAction.Invoke(client);

                    if (template.DestroyOnUse)
                    {
                        await plInvent.DestroyItem(useItemID);
                    }

                    break;
                case "give": // give
                    if (!cmdParser.HasNextToken())
                    {
                        ChatHandler.SendCommandUsageText(client, "/inv(entory) give [PartOfName/PlayerID] [ItemID] [Amount (default = 1)]");
                        return;
                    }

                    var otherClient = ClientMethods.FindClient(cmdParser.GetNextToken());
                    if (otherClient == null)
                    {
                        ChatHandler.SendCommandErrorText(client, "We couldn't find that player.");
                        return;
                    }

                    /*if (client == otherClient)
                    {
                        ChatHandler.SendCommandErrorText(client, "You can't use this command to yourself.");
                        return;
                    }*/

                    if (!client.IsLoggedIn() || !client.HasActiveChar())
                    {
                        ChatHandler.SendCommandErrorText(client, "That player is not logged in.");
                        return;
                    }

                    if (!cmdParser.HasNextToken(typeof(int)))
                    {
                        ChatHandler.SendCommandUsageText(client, "/inv(entory) give PartOfName/PlayerID [ItemID] [Amount (default = 1)]");
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

                    var amount = 1;
                    if (cmdParser.HasNextToken(typeof(int)))
                    {
                        amount = int.Parse(cmdParser.GetNextToken());
                    }

                    if (amount <= 0)
                    {
                        ChatHandler.SendCommandErrorText(client, "That's an invalid amount.");
                        return;
                    }

                    var playerInv = client.GetActiveChar().Inventory;
                    if (!playerInv.HasItem(giveItemID))
                    {
                        ChatHandler.SendCommandErrorText(client, "You don't have that item.");
                        return;
                    }

                    var giveItemTemplate = ItemTemplate.GetTemplate(giveItemID);

                    if (!playerInv.HasItem(giveItemID, amount))
                    {
                        ChatHandler.SendCommandErrorText(client, $"You don't have that much/many {giveItemTemplate.Name}/(s).");
                        return;
                    }

                    var otherInv = client.GetActiveChar().Inventory;

                    if (!otherInv.CanAddItem(giveItemID, amount))
                    {
                        ChatHandler.SendCommandErrorText(client, $"You cannot give that item to that player. Too heavy for them.");
                        ChatHandler.SendCommandErrorText(otherClient, $"An item was given to you but you cannot recieve it (weight limitation).");
                        return;
                    }

                    await playerInv.DestroyItem(giveItemID, amount);
                    await otherInv.SpawnItem(otherClient.GetActiveChar(), giveItemID, amount);

                    ChatHandler.SendCommandSuccessText(client, $"You have given ({amount}) {giveItemTemplate.Name} to {otherClient.Name}.");
                    ChatHandler.SendCommandSuccessText(client, $"You were given ({amount}) {giveItemTemplate.Name} from {client.Name}.");
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
            foreach (var item in inventory.GetItems())
            {
                var itemInfo = ItemTemplate.GetTemplate(item.ItemID);
                ChatHandler.SendClientMessage(client,
                    $"ItemID: {item.ItemID} => {itemInfo.Name}, {itemInfo.Desc}, {itemInfo.GetItemType().ToString()}, {item.Amount}");
            }

            ChatHandler.SendClientMessage(client, "-----------------------------------");
        }
    }
}