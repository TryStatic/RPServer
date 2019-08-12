using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
        public static HashSet<ItemTemplate> ItemTemplatesList;

        public Action<Client> SelfAction;

        // id
        public string Name { set; get; }
        public string Desc { set; get; }
        public int Type { get; set; }
        public bool Tradeable { get; set; }
        public bool DestroyOnUse { get; set; }

        public static async Task LoadItemTemplates()
        {
            if (ItemTemplatesList != null)
            {
                Logger.GetInstance().ServerError("Items template has already been initiated.");
                return;
            }

            Logger.GetInstance().ServerInfo("Loading Item Templates from the database.");

            ItemTemplatesList = new HashSet<ItemTemplate>();

            const string query = "SELECT * FROM itemstemplate";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync<ItemTemplate>(query);
                    ItemTemplatesList = new HashSet<ItemTemplate>();
                    foreach (var i in result)
                    {
                        var newItemTemplate = new ItemTemplate
                        {
                            ID = i.ID,
                            Name = i.Name,
                            Desc = i.Desc,
                            Type = i.Type,
                            SelfAction = GetItemAction(i.ID)
                        };
                        ItemTemplatesList.Add(newItemTemplate);
                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }

        public static bool IsValidItemTemplateID(int itemTemplateID)
        {
            return ItemTemplatesList.FirstOrDefault(it => it.ID == itemTemplateID) != null;
        }

        public static ItemTemplate GetTemplate(int itemID)
        {
            var itemTemplate = ItemTemplatesList.FirstOrDefault(it => it.ID == itemID);
            if (itemTemplate == null)
                Logger.GetInstance()
                    .ServerError("ItemTemplate.GetTemplate was not passed a valid template ID, returning null.");
            return itemTemplate;
        }

        public ItemType GetItemType()
        {
            return (ItemType) Type;
        }

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
            if (client == null) return;
            ChatHandler.SendClientMessage(client, $"You rolled {RandomGenerator.GetInstance().Next(0, 7)}");
        }

        #endregion
    }

    internal enum ItemType
    {
        General, // 0
        Currency // 1
    }
}