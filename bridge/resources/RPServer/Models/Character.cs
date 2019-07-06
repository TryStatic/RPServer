using System.Collections.Generic;
using System.Threading.Tasks;
using RPServer.Models.CharacterHelpers;

namespace RPServer.Models
{
    internal class Character
    {
        public static readonly string DataKey = "ACTIVE_CHARACTER_DATA";

        public CharacterDbData DbData { set; get; }
        public SkinCustomization SkinCustomization { set; get; }

        private Character(CharacterDbData dbData, SkinCustomization customSkin)
        {
            DbData = dbData;
            SkinCustomization = customSkin;
        }


        #region DATABASE
        public static async Task CreateNewAsync(Account account, string charName)
        {
            var newDbData = new CharacterDbData(account, charName)
            {
                Customization = new SkinCustomization().Serialize()
            };
            await newDbData.CreateAsync();
        }
        public static async Task<List<Character>> FetchAllAsync(Account account)
        {
            var chars = new List<Character>();

            var dbDataList = await CharacterDbData.ReadByAccountAsync(account);
            foreach (var charDbData in dbDataList)
            {
                chars.Add(new Character(charDbData, SkinCustomization.Deserialize(charDbData.Customization)));
            }
            return chars;
        }
        public static async Task<Character> FetchAsync(int charId)
        {
            var dbData = await CharacterDbData.ReadAsync(charId);
            var customSkin = SkinCustomization.Deserialize(dbData.Customization);
            return new Character(dbData, customSkin);
        }
        public static async Task<bool> ExistsAsync(int sqlId)
        {
            if (sqlId < 0) return false;

            var character = await CharacterDbData.ReadAsync(sqlId);
            return character != null;
        }
        public async Task SaveAsync()
        {
            DbData.Customization = SkinCustomization.Serialize();
            await DbData.UpdateAsync();
        }
        public async Task DeleteAsync(int charId)
        {
            await DbData.DeleteAsync();
        }
        #endregion
    }
}
