using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;

namespace RPServer.Models.Inventory.Template
{
    internal class MultitionItemTemplate : ItemTemplate
    {
        private MultitionItemTemplate(string itemName, string itemDesc) : base(itemName, itemDesc)
        {
        }

        internal static void LoadTemplates()
        {
            if (Loaded) return;
            var count = ItemTemplates.Count;

            ItemTemplates.Add(new MultitionItemTemplate("Dice", "Just an ordinary Dice")
            {
                Weight = 20.0f,
                ItemAction = args => // (Client client)
                {
                    if (args == null || args.Length < 1) return;
                    var client = args[0] as Client;

                    if (client == null) return;
                    if (!client.IsLoggedIn() || !client.HasActiveChar()) return;
                    // TODO: Check whether the active character's inventory actually has the item they are trying to use.

                    ChatHandler.SendActionMessage(client, Shared.Data.Chat.NormalChatMaxDistance, $"rolls a dice landing on {RandomGenerator.GetInstance().Next(0, 7)}");
                }
            });

            Logger.GetInstance().ServerInfo($"\t\tLoaded {ItemTemplates.Count - count} Multition item Templates...");

        }
    }
}