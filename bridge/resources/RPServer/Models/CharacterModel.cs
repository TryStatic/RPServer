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

        public CharacterModel()
        {
        }
        public CharacterModel(Account owner, string name)
        {
            CharOwnerID = owner.DbData.AccountID;
            CharacterName = name;
        }

    }
}
