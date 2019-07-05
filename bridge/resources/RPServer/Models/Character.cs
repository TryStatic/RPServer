using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RPServer.Database;
using RPServer.Models.CharacterHelpers;
using RPServer.Models.Helpers;
using RPServer.Util;

namespace RPServer.Models
{
    internal class Character : SaveableData
    {
        public static readonly string DataKey = "ACTIVE_CHARACTER_DATA";
        private SkinCustomization _skinCustomization;

        #region SQL_SAVEABLE_DATA

        [SqlColumnName("characterid")]
        public int SqlId { get; }
        [SqlColumnName("charname")]
        public string Name { set; get; }

        [SqlColumnName("customization")]
        public SkinCustomization SkinCustomization
        {
            set => _skinCustomization = value;
            get => _skinCustomization ?? (_skinCustomization = new SkinCustomization());
        }

        #endregion

        public Account Owner { get; set; }

        private Character(int sqlId)
        {
            SqlId = sqlId;
        }

        #region DATABASE
        public static async Task CreateNewAsync(Account account, string charName)
        {
            if (await ExistsAsync(await GetSqlIdAsync(charName)))
                return;

            const string query = "INSERT INTO characters(charownerID, charname) VALUES (@accownerID, @charname)";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@accownerID", account.SqlId);
                    cmd.Parameters.AddWithValue("@charname", charName);
                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
                }
            }
        }
        public static async Task<List<Character>> FetchAllAsync(Account account)
        {
            const string query = "SELECT characterID, charname, customization FROM characters WHERE charownerID = @accountID";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@accountID", account.SqlId);
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
                                SkinCustomization = JsonConvert.DeserializeObject<SkinCustomization>(r.GetStringExtended("customization"))
                            };
                            chars.Add(fetchedChar);
                        }

                        return chars;

                    }
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
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

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@characterid", charId);
                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (!await r.ReadAsync())
                            return null;

                        var fetched = new Character(charId)
                        {
                            Name = r.GetStringExtended("charname"),
                            SkinCustomization = JsonConvert.DeserializeObject<SkinCustomization>(r.GetStringExtended("customization"))
                        };
                        return fetched;

                    }
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
                }

                return null;
            }
        }
        public static async Task<int> FetchCount(Account account)
        {
            if (account == null) return -1;

            const string query = "SELECT COUNT(characterID) FROM characters WHERE charownerID = @charownerID";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@charownerID", account.SqlId);
                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        if (!r.Read()) return -1;
                        var count = r.GetInt32(0);
                        return count;
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
                }
            }

            throw new Exception("There was an error in [Character.ExistsAsync]");
        }

        public static async Task<bool> ExistsAsync(int sqlId)
        {

            if (sqlId < 0) return false;

            const string query = "SELECT characterID FROM characters WHERE characterID = @sqlid";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@sqlid", sqlId);
                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        return await r.ReadAsync() && r.HasRows;
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
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

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@characterID", SqlId);

                    cmd.Parameters.AddWithValue("@charname", Name);
                    cmd.Parameters.AddWithValue("@customization", JsonConvert.SerializeObject(SkinCustomization));

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
                }
            }
        }
        public async Task SaveSingleAsync<T>(Expression<Func<T>> expression) // Skin customization???
        {
            var column = GetColumnName(expression, out var value);
            if (string.IsNullOrWhiteSpace(column))
                throw new Exception("Invalid Column Name");

            var query = $"UPDATE characters SET {column} = @value WHERE characterID = @sqlId";


            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@sqlId", SqlId);
                    cmd.Parameters.AddWithValue("@value", value);

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
                }
            }
        }
        public static async Task DeleteAsync(int charId)
        {
            const string query = "DELETE FROM characters WHERE characterID = @charId";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@characterID", charId);

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
                }
            }
        }
        public static async Task<int> GetSqlIdAsync(string charName)
        {
            const string query = "SELECT characterID FROM characters WHERE charname = @name LIMIT 1";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@name", charName);

                    await dbConn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            return -1;

                        var sqlId = reader.GetInt32Extended("characterID");
                        return sqlId;
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
                }

                return -1;
            }
        }
        #endregion
    }
}
