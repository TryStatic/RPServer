using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using RPServer.Util;

namespace RPServer.Models
{
    [Table("characters")]
    internal class Character : Model<Character>
    {
        public int CharOwnerID { set; get; }
        public string CharacterName { set; get; }

        public Appearance Appearance;
        public HashSet<Alias> Aliases = new HashSet<Alias>();
        public int AltIdentifier = RandomGenerator.GetInstance().UniqueNext();

        public Character()
        {
            
        }

        /// <summary>
        /// Use to create new character
        /// </summary>
        public static async Task CreateNewAsync(Account charOwner, string newCharName)
        {
            var newChar = new Character()
            {
                CharOwnerID = charOwner.ID,
                CharacterName = newCharName
            };
            await newChar.CreateAsync();
        }
        public static async Task<List<Character>> FetchAllAsync(Account account)
        {
            var result = await ReadByKeyAsync(() => new Character().CharOwnerID, account.ID);
            var charsData = result.ToList();
            return charsData;
        }
        public async Task<Appearance> GetAppearance()
        {
            var app =  await Appearance.ReadByKeyAsync(() => new Appearance().CharacterID, this.ID);
            return app.FirstOrDefault();
        }

        #region Alias
        public async Task<HashSet<Alias>> GetAliases() => await Alias.FetchAllByChar(this);
        #endregion

        public async Task SaveAll()
        {
            await UpdateAsync(this); // Update character
            await Appearance.UpdateAsync(Appearance); // Update Appearance
            await UpdateAlises();
        }


        public async Task UpdateAlises()
        {
            var dbRecords = await Alias.FetchAllByChar(this);

            foreach (var dbRec in dbRecords)
            {
                if (Aliases.Contains(dbRec))
                {
                    await Alias.UpdateAlias(Aliases.First(r => r.Equals(dbRec)));
                    Aliases.Remove(dbRec);
                }
                else
                {
                    await Alias.DeleteAlias(Aliases.First(r => r.Equals(dbRec)));
                }
            }
            
            foreach (var i in Aliases)
            {
                await Alias.CreateAsync(i.CharID, i.AliasedID, i.AliasName, i.AliasDesc);
            }
        }
    }
}
