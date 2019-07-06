using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using RPServer.Database;

namespace RPServer.Models
{
    [Table("characters")]
    internal class CharacterDbData
    {
        [Key]
        public int CharacterID { get; set; }
        public int CharOwnerID { set; get; }
        public string CharacterName { set; get; }
        public string Customization { set; get; }

        private CharacterDbData()
        {
        }

        public CharacterDbData(Account owner, string name)
        {
            CharOwnerID = owner.DbData.AccountID;
            CharacterName = name;
        }


        public static async Task<int> CreateAsync(CharacterDbData newData)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.InsertAsync(newData);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return -1;
            }
        }
        public async Task<int> CreateAsync() => await CreateAsync(this);

        public static async Task<CharacterDbData> ReadAsync(int sqlID)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.GetAsync<CharacterDbData>(sqlID);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }
        public static async Task<List<CharacterDbData>> ReadByAccountAsync(Account account)
        {
            const string query = "SELECT * FROM characters WHERE CharOwnerID = @accountID";

            {
                using (var dbConn = DbConnectionProvider.CreateDbConnection())
                {
                    try
                    {
                        var result = await dbConn.QueryAsync<CharacterDbData>(query, new { accountID = account.DbData.AccountID });
                        return result.ToList();
                    }
                    catch (DbException ex)
                    {
                        DbConnectionProvider.HandleDbException(ex);
                    }

                }
            }
            return null;
        }

        public static async Task<bool> UpdateAsync(CharacterDbData dbAcc)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.UpdateAsync(dbAcc);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
            }
        }
        public async Task<bool> UpdateAsync() => await UpdateAsync(this);

        public static async Task<bool> DeleteAsync(CharacterDbData dbAcc)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.DeleteAsync(dbAcc);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
            }
        }
        public async Task<bool> DeleteAsync() => await DeleteAsync(this);

    }
}
