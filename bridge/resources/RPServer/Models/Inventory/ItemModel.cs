using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using RPServer.Database;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    internal class ItemModel
    {
        public int Amount;
        public int ContainerID; // Key 3
        public int ItemID; // key 1
        public int OwnerID; // key 2

        public static async Task<HashSet<ItemModel>> LoadInventoryItems(CharacterModel character)
        {
            const string query = "SELECT * FROM items WHERE OwnerID = ownerid AND ContainerID = containerid";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync(query,
                        new {ownerid = character.ID, containerid = (int) ContainerType.CharacterInventory});
                    var itemList = new HashSet<ItemModel>();
                    foreach (var i in result)
                        itemList.Add(new ItemModel
                        {
                            ItemID = i.ItemID,
                            OwnerID = i.OwnerID,
                            ContainerID = i.ContainerID,
                            Amount = i.Amount
                        });
                    return itemList;
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }

        public static async Task<HashSet<ItemModel>> LoadInventoryItems(VehicleModel vehicle,
            VehicleContainer container)
        {
            const string query = "SELECT * FROM items WHERE OwnerID = ownerid AND ContainerID = containerid";

            ContainerType actualContainer;
            switch (container)
            {
                case VehicleContainer.Trunk:
                    actualContainer = ContainerType.VehicleTrunk;
                    break;
                case VehicleContainer.Glovebox:
                    actualContainer = ContainerType.VehicleGlovebox;
                    break;
                default:
                    Logger.GetInstance().ServerError("Invalid VehicleContainer");
                    return null;
            }

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync(query,
                        new {ownerid = vehicle.ID, containerid = (int) actualContainer});
                    var itemList = new HashSet<ItemModel>();
                    foreach (var i in result)
                        itemList.Add(new ItemModel
                        {
                            ItemID = i.ItemID,
                            OwnerID = i.OwnerID,
                            ContainerID = i.ContainerID,
                            Amount = i.Amount
                        });
                    return itemList;
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }

        public static async Task<HashSet<ItemModel>> LoadWorldItems()
        {
            const string query = "SELECT * FROM items WHERE ContainerID = containerid";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync(query, new {containerid = (int) ContainerType.WorldInventory});
                    var itemList = new HashSet<ItemModel>();
                    foreach (var i in result)
                        itemList.Add(new ItemModel
                        {
                            ItemID = i.ItemID,
                            OwnerID = i.OwnerID,
                            ContainerID = i.ContainerID,
                            Amount = i.Amount
                        });
                    return itemList;
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }

        private enum ContainerType
        {
            CharacterInventory, // 0
            VehicleTrunk, // 1
            VehicleGlovebox, // 2
            WorldInventory // 3 - Dropped Items
        }

        internal enum VehicleContainer
        {
            Trunk,
            Glovebox
        }
    }
}