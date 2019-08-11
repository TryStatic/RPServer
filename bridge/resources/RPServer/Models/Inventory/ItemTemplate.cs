using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.Database;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    [Table("itemstemplate")]
    internal class ItemTemplate : Model<ItemTemplate>
    {
        // id
        public string Name { set; get; }
        public string Desc { set; get; }
        public int Type { get; set; }
        public Action<Client> Action;

        public static HashSet<ItemTemplate> ItemsTemplate;

        public static async Task LoadItemTemplates()
        {
            if (ItemsTemplate != null)
            {
                Logger.GetInstance().ServerError("Items template has already been initiated.");
                return;
            }

            Logger.GetInstance().ServerInfo("Loading Item Templates from the database.");

            ItemsTemplate = new HashSet<ItemTemplate>();

            const string query = "SELECT * FROM itemstemplate";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync<ItemTemplate>(query);
                    ItemsTemplate = new HashSet<ItemTemplate>();
                    foreach (var i in result)
                    {
                        var newItemTemplate = new ItemTemplate
                        {
                            ID = i.ID,
                            Name = i.Name,
                            Desc = i.Desc,
                            Type = i.Type,
                            Action = GetItemAction(i.ID)
                        };
                        ItemsTemplate.Add(newItemTemplate);
                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }

        public ItemType GetItemType() => Enum.Parse<ItemType>(Type.ToString());

        #region ItemAcions
        private static Action<Client> GetItemAction(int id)
        {
            switch (id)
            {
                case 2: // Dice
                    return DiceRollAction;
                default:
                    return null;
            }
        }

        private static void DiceRollAction(Client client)
        {
            if(client == null) return;
            ChatHandler.SendClientMessage(client, $"You rolled {RandomGenerator.GetInstance().Next(0,7)}");
        }
        #endregion
    }

    internal enum ItemType
    {
        General, // 0
        Currency // 1
    }
}
