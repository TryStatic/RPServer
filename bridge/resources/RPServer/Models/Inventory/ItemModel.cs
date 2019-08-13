using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using GTANetworkAPI;
using RPServer.Database;
using RPServer.Util;
using Object = GTANetworkAPI.Object;

namespace RPServer.Models.Inventory
{
    internal class ItemModel
    {
        public static int LastDroppedItemID;

        public int ContainerID { set; get; } // Key 3
        public int ItemID { set; get; } // key 1
        public int OwnerID { set; get; } // key 2
        public int Amount { set; get; }
        public uint Dimension { set; get; }
        public float PosX { set; get; }
        public float PosY { set; get; }
        public float PosZ { set; get; }
        public float RotX { set; get; }
        public float RotY { set; get; }
        public float RotZ { set; get; }

        public Object EntityID;
        public TextLabel TextLabel;

        public ItemModel()
        {
        }

        public ItemModel(int itemID, int ownerID, ContainerType container, int amount = 1)
        {
            ItemID = itemID;
            OwnerID = ownerID;
            ContainerID = (int) container;
            Amount = amount;

        }

        public static async Task<HashSet<ItemModel>> LoadInventoryItems(CharacterModel character)
        {
            const string query = "SELECT * FROM items WHERE OwnerID = @ownerid AND ContainerID = @containerid";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync(query, new {ownerid = character.ID, containerid = (int) ContainerType.CharacterInventory});
                    var itemList = new HashSet<ItemModel>();

                    foreach (var i in result)
                    {
                        itemList.Add(new ItemModel
                        {
                            ItemID = i.ItemID,
                            OwnerID = i.OwnerID,
                            ContainerID = i.ContainerID,
                            Amount = i.Amount,
                            Dimension = i.Dimension,
                            PosX = i.PosX,
                            PosY = i.PosY,
                            PosZ = i.PosZ,
                            RotX = i.RotX,
                            RotY = i.RotY,
                            RotZ = i.RotZ
                        });
                    }


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
            const string query = "SELECT * FROM items WHERE OwnerID = @ownerid AND ContainerID = @containerid";

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
            const string query = "SELECT * FROM items WHERE ContainerID = @containerid";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync(query, new {containerid = (int) ContainerType.WorldInventory});
                    var itemList = new HashSet<ItemModel>();
                    foreach (var i in result)
                    {
                        var newItem = new ItemModel
                        {
                            ItemID = i.ItemID,
                            OwnerID = i.OwnerID,
                            ContainerID = i.ContainerID,
                            Amount = i.Amount,
                            Dimension = i.Dimension,
                            PosX = i.PosX,
                            PosY = i.PosY,
                            PosZ = i.PosZ,
                            RotX = i.RotX,
                            RotY = i.RotY,
                            RotZ = i.RotZ
                        };

                        if ((int) ContainerType.WorldInventory == newItem.ContainerID)
                        {
                            LastDroppedItemID = Math.Min(LastDroppedItemID, newItem.OwnerID);

                            var template = ItemTemplate.GetTemplate(newItem.ItemID);

                            var itemPos = new Vector3(newItem.PosX, newItem.PosY, newItem.PosZ);
                            var itemRot = new Vector3(newItem.RotX, newItem.RotY, newItem.RotZ);
                            newItem.EntityID = NAPI.Object.CreateObject(template.ObjectID, itemPos, itemRot, 255, newItem.Dimension);
                            itemPos.Z += 0.2f;
                            newItem.TextLabel = NAPI.TextLabel.CreateTextLabel($"{template.Name} ({newItem.Amount})", itemPos, 5.0f, 0.5f, (int)Shared.Enums.Font.ChaletLondon, new Color(255, 255, 255), false, newItem.Dimension);
                        }

                        itemList.Add(newItem);
                    }

                    return itemList;
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }

        public enum ContainerType
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

        public async Task Delete()
        {
            const string query = "DELETE FROM items WHERE ItemID = @itemid AND OwnerID = @ownerid AND ContainerID = @containerid";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new { itemid = ItemID, ownerid = OwnerID, containerid = ContainerID });
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }

        public async Task Update()
        {
            const string query = "UPDATE items " +
                                 " SET Amount = @amount " +
                                 " WHERE ItemID = @itemid AND OwnerID = @ownerid AND ContainerID = @containerid";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new { itemid = ItemID, ownerid = OwnerID, containerid = ContainerID, amount = Amount });
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }

        public async Task Create()
        {
            const string query =
                "INSERT INTO items(ItemID, OwnerID, ContainerID, Amount) VALUES (@itemid, @ownerid, @containerid, @amount)";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new
                    {
                        itemid = this.ItemID,
                        ownerid = this.OwnerID,
                        containerid = this.ContainerID,
                        amount = this.Amount
                    });
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
    }
}