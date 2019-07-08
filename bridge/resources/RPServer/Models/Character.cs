using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using RPServer.Models.CharacterHelpers;

namespace RPServer.Models
{
    [Table("characters")]
    internal class Character : Model<Character>
    {
        public static readonly string DataKey = "ACTIVE_CHARACTER_DATA";
        [Key]
        public int CharacterID { get; set; }
        public int CharOwnerID { set; get; }
        public string CharacterName { set; get; }
        public string Customization
        {
            get => CustomSkin.Serialize();
            private set => CustomSkin = SkinCustomization.Deserialize(value);
        }

        public SkinCustomization CustomSkin;

        public static async Task CreateNewAsync(Account charOwner, string newCharName)
        {
            var newChar = new Character()
            {
                CharOwnerID = charOwner.AccountID,
                CharacterName = newCharName,
                CustomSkin = new SkinCustomization()
            };
            await newChar.CreateAsync();
        }

        public static async Task<List<Character>> FetchAllAsync(Account account)
        {
            var result = await ReadByKeyAsync(() => new Character().CharOwnerID, account.AccountID);
            var charsData = result.ToList();
            return charsData;
        }

        public static async Task<Character> FetchAsync(int charId)
        {
            return await ReadAsync(charId);
        }


        public static async Task<bool> ExistsAsync(int sqlId)
        {
            if (sqlId < 0) return false;
            var character = await ReadAsync(sqlId);
            return character != null;
        }

        public async Task SaveAsync()
        {
            await UpdateAsync();
        }
    }
}
