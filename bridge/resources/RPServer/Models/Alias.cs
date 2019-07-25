using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RPServer.Database;

namespace RPServer.Models
{
    internal class Alias
    { 
        public int CharID { set; get; }
        public int AliasedID { set; get; }
        public string AliasName { set; get; }
        public string AliasDesc { set; get; }

        private Alias(Character character, Character aliasedCharacter, string aliasName, string aliasDesc = "")
        {
            CharID = character.ID;
            AliasedID = aliasedCharacter.ID;
            AliasName = aliasName;
            AliasDesc = aliasDesc;
        }

        private Alias(int charid, int aliased, string aliasName, string aliasDesc = "")
        {
            CharID = charid;
            AliasedID = aliased;
            AliasName = aliasName;
            AliasDesc = aliasDesc;
        }

        public static async Task<bool> CreateAsync(Character character, Character aliasedCharacter, string aliasName, string aliasDesc = "")
        {
            const string query = "INSERT INTO aliases(CharID, AliasedID, AliasName, AliasDesc) VALUES (@CharID, @AliasedID, @AliasName, @AliasDesc)";

            var newAlias = new Alias(character, aliasedCharacter, aliasName, aliasDesc);

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new
                    {
                        CharID = newAlias.CharID,
                        AliasedID = newAlias.AliasedID,
                        AliasName = newAlias.AliasName,
                        AliasDesc = newAlias.AliasDesc == "" ? null : newAlias.AliasDesc
                });
                    return true;
                }
                catch(DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
            return false;
        }

        public static async Task<bool> HasAliasedTarget(Character character, Character target)
        {
            const string query = "SELECT * FROM aliases WHERE charID = @charID AND AliasedID = @aliasedID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result =
                        await dbConn.QueryAsync<int>(query, new {charID = character.ID, aliasedID = target.ID});
                    return result.Any();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
            }
        }

        public static async Task<Alias> FetchAlias(Character character, Character target)
        {
            const string query = "SELECT * FROM aliases WHERE charID = @charID AND AliasedID = @aliasedID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync(query, new { charID = character.ID, aliasedID = target.ID });
                    var unwrapped = result.SingleOrDefault();

                    if (unwrapped == null) return null;
                    return new Alias(unwrapped.CharID, unwrapped.AliasedID, unwrapped.AliasName, unwrapped.AliasDesc);

                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }


        public static async Task<List<Alias>> FetchAllByChar(Character character)
        {
            const string query = "SELECT * FROM aliases WHERE charID = @charID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync(query, new { charID = character.ID });
                    var aliases = new List<Alias>();
                    var aliasesDyn = result.ToList();
                    foreach (var al in aliasesDyn) aliases.Add(new Alias(al.CharID, al.AliasedID, al.AliasName, al.AliasDesc));
                    return aliases;
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }

        public static async Task<bool> UpdateAlias(Alias alias)
        {
            var query = "UPDATE aliases SET aliasName = @aliasname, aliasDesc = @aliasdesc WHERE CharID = @charID AND AliasedID = @aliasedID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new
                    {
                        aliasname = alias.AliasName,
                        aliasdesc = alias.AliasDesc,
                        charID = alias.CharID,
                        aliasedID = alias.AliasedID
                    });

                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
            }
        }
        
        public async Task<bool> UpdateAlias() => await UpdateAlias(this);

        public static async Task<bool> DeleteAlias(Alias alias)
        {
            var query = "DELETE FROM aliases WHERE CharID = @charID AND AliasedID = @aliasedID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new
                    {
                        charID = alias.CharID,
                        aliasedID = alias.AliasedID
                    });
                    return true;
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
            return false;
        }

        public async Task<bool> DeleteAlias() => await DeleteAlias(this);
    }
}
