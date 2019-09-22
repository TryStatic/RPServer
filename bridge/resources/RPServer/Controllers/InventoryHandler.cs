using GTANetworkAPI;
using RPServer.Controllers.Util;
using RPServer.InternalAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Models.Inventory.Item;
using RPServer.Models.Inventory.Template;

namespace RPServer.Controllers
{
    internal class InventoryHandler : Script
    {
        [Command("spawnitem", GreedyArg = true)]
        public void CMD_SpawnItem(Client client, string args = "") // (uint templateID, string target, uint count = 1)
        {
            var cmdParser = new CommandParser(args);

            if (cmdParser.HasNextToken())
            {
                var tok = cmdParser.GetNextToken();
                var success = uint.TryParse(tok, out var templateID);
                if (success)
                {
                    var template = ItemTemplate.GetTemplate(templateID);
                    if (template != null)
                    {
                        if (cmdParser.HasNextToken())
                        {
                            var target = cmdParser.GetNextToken();
                            var targetClient = ClientMethods.FindClient(target);
                            if (targetClient != null && targetClient.IsLoggedIn() && targetClient.HasActiveChar())
                            {
                                if (!cmdParser.HasNextToken())
                                {
                                    switch (template)
                                    {
                                        case MultitionItemTemplate multitionItemTemplate:
                                            targetClient.GetActiveChar().Inventory.SpawnItem(multitionItemTemplate);
                                            break;
                                        case SingletonItemTemplate singletonItemTemplate:
                                            targetClient.GetActiveChar().Inventory.SpawnItem(singletonItemTemplate);
                                            break;
                                        case StackableItemTemplate stackableItemTemplate:
                                            targetClient.GetActiveChar().Inventory.SpawnItem(stackableItemTemplate, 1);
                                            break;
                                    }
                                }
                                else
                                {
                                    var tok3 = cmdParser.GetNextToken();
                                    if (uint.TryParse(tok3, out var count))
                                    {
                                        switch (template)
                                        {
                                            case MultitionItemTemplate multitionItemTemplate:
                                                targetClient.GetActiveChar().Inventory.SpawnItem(multitionItemTemplate);
                                                break;
                                            case SingletonItemTemplate singletonItemTemplate:
                                                targetClient.GetActiveChar().Inventory.SpawnItem(singletonItemTemplate);
                                                break;
                                            case StackableItemTemplate stackableItemTemplate:
                                                targetClient.GetActiveChar().Inventory
                                                    .SpawnItem(stackableItemTemplate, count);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        ChatHandler.SendCommandErrorText(client, "Invalid Count");
                                    }
                                }
                            }
                            else
                            {
                                ChatHandler.SendCommandErrorText(client, "Invalid Target");
                            }
                        }
                        else
                        {
                            ChatHandler.SendCommandUsageText(client, $"/spawnitem {templateID} [Target] [Count]");
                        }
                    }
                    else
                    {
                        ChatHandler.SendCommandErrorText(client, "Invalid Template ID");
                    }
                }
                else
                {
                    ChatHandler.SendCommandUsageText(client, "/spawnitem [templateID] [Target] [Count]");
                }
            }
            else
            {
                ChatHandler.SendClientMessage(client, "Item Templates: ");
                foreach (var t in ItemTemplate.GetTemplates())
                {
                    ChatHandler.SendClientMessage(client, $"~: ID: {t.ID}, Name: {t.ItemName}, Desc: {t.ItemDesc}, Usable: {t.IsUsable()}");
                }
                ChatHandler.SendClientMessage(client, "/spawnitem [templateID] [Target] [Count]");
            }
        }

        [Command("inventory", Alias = "inv", GreedyArg = true)]
        public void CMD_Inventory(Client client, string args = "")
        {
            var cmdParser = new CommandParser(args);

            if (!cmdParser.HasNextToken())
            {
                ChatHandler.SendCommandUsageText(client, "/inv(entory) [list]");
                return;
            }

            switch (cmdParser.GetNextToken())
            {
                case "list":
                {
                    ChatHandler.SendClientMessage(client, "Your items: ");
                    foreach (var item in client.GetActiveChar().Inventory.GetItems())
                    {
                        switch (item)
                        {
                            case NonStackableItem nonStackableItem:
                                ChatHandler.SendClientMessage(client, $"~: ID: {item.ID}, Name: {item.Template.ItemName}, {item.Template.ItemDesc}");
                                break;
                            case StackableItem stackableItem:
                                ChatHandler.SendClientMessage(client, $"~: ID: {item.ID}, Name: {item.Template.ItemName} [{stackableItem.Count}], {item.Template.ItemDesc}");
                                break;
                        }
                    }
                    ChatHandler.SendClientMessage(client, "-------");
                    break;
                }
                default:
                {
                    ChatHandler.SendCommandUsageText(client, "/inv(entory) [list]");
                    break;
                }
            }
        }

    }
}
