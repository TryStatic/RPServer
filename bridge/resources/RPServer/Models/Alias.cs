using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RPServer.Database;

namespace RPServer.Models
{
    internal class Alias
    { // Aliases use a Composite key and had to hard-implemented
        public int CharID { set; get; } // Key 1
        public int AliasedID { set; get; } // Key 2
        public string AliasName { set; get; }
        public string AliasDesc { set; get; }

        public Alias(Character character, Character aliasedCharacter, string aliasName, string aliasDesc = "")
        {
            CharID = character.ID;
            AliasedID = aliasedCharacter.ID;
            AliasName = aliasName;
            AliasDesc = aliasDesc;
        }
        public Alias(int charid, int aliased, string aliasName, string aliasDesc = "")
        {
            CharID = charid;
            AliasedID = aliased;
            AliasName = aliasName;
            AliasDesc = aliasDesc;
        }

        public static async Task<bool> CreateAsync(int charid, int alisedid, string aliasName, string aliasDesc = "")
        {
            const string query = "INSERT INTO aliases(CharID, AliasedID, AliasName, AliasDesc) VALUES (@CharID, @AliasedID, @AliasName, @AliasDesc)";

            var newAlias = new Alias(charid, alisedid, aliasName, aliasDesc);

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
        public static async Task<bool> Exist(int charid, int alisedID)
        {
            const string query = "SELECT * FROM aliases WHERE charID = @charID AND AliasedID = @aliasedID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result =
                        await dbConn.QueryAsync<int>(query, new {charID = charid, aliasedID = alisedID});
                    return result.Any();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
            }
        }
        public static async Task<HashSet<Alias>> FetchAllByChar(Character character)
        {
            const string query = "SELECT * FROM aliases WHERE charID = @charID";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                { // cumhere
                    var result = await dbConn.QueryAsync(query, new { charID = character.ID });
                    var aliases = new HashSet<Alias>();
                    foreach (var i in result) aliases.Add(new Alias(i.CharID, i.AliasedID, i.AliasName, i.AliasDesc));
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
        public static async Task UpdateAllByChar(HashSet<Alias> aliasesRef, Character character)
        {
            var aliases = aliasesRef.ToHashSet();
            var dbRecords = await FetchAllByChar(character);

            foreach (var dbRec in dbRecords)
            {
                if (aliases.Contains(dbRec))
                {
                    await UpdateAlias(aliases.First(r => r.Equals(dbRec)));
                    aliases.Remove(dbRec);
                }
                else
                {
                    await DeleteAlias(aliases.First(r => r.Equals(dbRec)));
                }
            }

            foreach (var i in aliases)
            {
                await CreateAsync(i.CharID, i.AliasedID, i.AliasName, i.AliasDesc);
            }
        }

        #region Generated
        protected bool Equals(Alias other)
        {
            return CharID == other.CharID && AliasedID == other.AliasedID;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Alias) obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (CharID * 397) ^ AliasedID;
            }
        }
        public static bool operator ==(Alias left, Alias right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Alias left, Alias right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
