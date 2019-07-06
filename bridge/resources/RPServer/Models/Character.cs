using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using RPServer.Database;
using RPServer.Models.CharacterHelpers;
using RPServer.Util;

namespace RPServer.Models
{
    internal class Character
    {
        public static readonly string DataKey = "ACTIVE_CHARACTER_DATA";

        public int SqlId { get; }
        public string Name { set; get; }
        public SkinCustomization SkinCustomization { set; get; }
        public Account Owner { get; set; }

        private Character(int sqlId)
        {
            SqlId = sqlId;
            SkinCustomization = new SkinCustomization();
        }

        #region DATABASE
        public static async Task CreateNewAsync(Account account, string charName)
        {
            if (await ExistsAsync(await GetSqlIdAsync(charName)))
                return;

            const string query = "INSERT INTO characters(charownerID, charname, customization) VALUES (@accownerID, @charname, @customization)";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@accownerID", account.DbData.AccountID);
                    cmd.AddParameterWithValue("@charname", charName);
                    cmd.AddParameterWithValue("@customization", new SkinCustomization().Serialize());
                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
        public static async Task<List<Character>> FetchAllAsync(Account account)
        {
            const string query = "SELECT characterID, charname, customization FROM characters WHERE charownerID = @accountID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@accountID", account.DbData.AccountID);
                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        var chars = new List<Character>();
                        while (await r.ReadAsync())
                        {
                            var fetchedChar = new Character(r.GetInt32Extended("characterID"))
                            {
                                Owner = account,
                                Name = r.GetStringExtended("charname"),
                                SkinCustomization = SkinCustomization.Deserialize(r.GetStringExtended("customization"))
                            };
                            chars.Add(fetchedChar);
                        }

                        return chars;

                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }
        public static async Task<Character> FetchAsync(int charId)
        {
            const string query =
                "SELECT charname, customization FROM characters WHERE characterID = @characterid";

            if (!await ExistsAsync(charId))
                return null;

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@characterid", charId);
                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (!await r.ReadAsync())
                            return null;

                        var fetched = new Character(charId)
                        {
                            Name = r.GetStringExtended("charname"),
                            SkinCustomization = SkinCustomization.Deserialize(r.GetStringExtended("customization"))
                        };
                        return fetched;

                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }
        public static async Task<int> FetchCount(Account account)
        {
            if (account == null) return -1;

            const string query = "SELECT COUNT(characterID) FROM characters WHERE charownerID = @charownerID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@charownerID", account.DbData.AccountID);
                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        if (!r.Read()) return -1;
                        var count = r.GetInt32(0);
                        return count;
                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }

            throw new Exception("There was an error in [Character.ExistsAsync]");
        }
        public static async Task<bool> ExistsAsync(int sqlId)
        {

            if (sqlId < 0) return false;

            const string query = "SELECT characterID FROM characters WHERE characterID = @sqlid";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@sqlid", sqlId);
                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        return await r.ReadAsync() && r.HasRows;
                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }

            throw new Exception("There was an error in [Character.ExistsAsync]");
        }
        public async Task SaveAsync()
        {
            const string query = "UPDATE characters " +
                                 "SET charname = @charname, " +
                                 "customization = @customization " +
                                 "WHERE characterID = @characterID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@characterID", SqlId);

                    cmd.AddParameterWithValue("@charname", Name);
                    cmd.AddParameterWithValue("@customization", SkinCustomization.Serialize());

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
        public static async Task DeleteAsync(int charId)
        {
            const string query = "DELETE FROM characters WHERE characterID = @charId";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@characterID", charId);

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
        public static async Task<int> GetSqlIdAsync(string charName)
        {
            const string query = "SELECT characterID FROM characters WHERE charname = @name LIMIT 1";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@name", charName);

                    await dbConn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            return -1;

                        var sqlId = reader.GetInt32Extended("characterID");
                        return sqlId;
                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return -1;
            }
        }
        #endregion
    }
}
