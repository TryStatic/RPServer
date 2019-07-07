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
    internal class CharacterModel : Model<CharacterModel>
    {
        [Key]
        public int CharacterID { get; set; }
        public int CharOwnerID { set; get; }
        public string CharacterName { set; get; }
        public string Customization { set; get; }

        private CharacterModel()
        {
        }
        public CharacterModel(Account owner, string name)
        {
            CharOwnerID = owner.DbData.AccountID;
            CharacterName = name;
        }

        public static async Task<List<CharacterModel>> ReadByAccountAsync(Account account)
        {
            const string query = "SELECT * FROM characters WHERE CharOwnerID = @accountID";

            {
                using (var dbConn = DbConnectionProvider.CreateDbConnection())
                {
                    try
                    {
                        var result = await dbConn.QueryAsync<CharacterModel>(query, new { accountID = account.DbData.AccountID });
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

    }
}
