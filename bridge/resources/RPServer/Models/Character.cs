using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RPServer.Database;
using RPServer.Util;

namespace RPServer.Models
{
    internal class Character : SaveableData
    {
        public static readonly string DataKey = "ACTIVE_CHARACTER_DATA";

        #region SQL_SAVEABLE_DATA
        [SqlColumnName("characterid")]
        public int SqlId { get; }
        [SqlColumnName("name")]
        public string Name { set; get; }
        [SqlColumnName("skinmodel")]
        public int SkinModel { set; get; }
        #endregion

        public Account Owner { get; set; }

        private Character(int sqlId)
        {
            SqlId = sqlId;
        }

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
        public static async Task<List<Character>> FetchAsync(Account account)
        {
            throw new NotImplementedException();
        }
        public static async Task<Character> FetchAsync(int charId)
        {
            const string query = "SELECT characterID, charownerID, charname, skinmodel FROM characters WHERE characterID = @characterid";

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

                        var fetchedAcc = new Character(charId)
                        {
                            Name = r.GetStringExtended("charname"),
                            SkinModel = r.GetInt32Extended("skinmodel")
                        };
                        return fetchedAcc;

                    }
                }
                catch (MySqlException ex)
                {
                    Logger.GetInstance().MySqlError(ex.Message, ex.Code);
                }

                return null;
            }
        }
        public static async Task<bool> ExistsAsync(int sqlId)
        {
            const string query = "SELECT characterID FROM characters WHERE characerID = @sqlid";

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
            throw new NotImplementedException();
        }

        public async Task SaveSingleAsync<T>(Expression<Func<T>> expression)
        {
            throw new NotImplementedException();
        }
        public static async Task DeleteAsync(string username)
        {
            throw new NotImplementedException();
        }
        public static async Task<int> GetSqlIdAsync(string charName)
        {
            throw new NotImplementedException();
        }


    }




}
