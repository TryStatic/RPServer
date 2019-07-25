using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace RPServer.Models
{
    [Table("characters")]
    internal class Character : Model<Character>
    {
        public int CharOwnerID { set; get; }
        public string CharacterName { set; get; }

        public Appearance Appearance;
        public List<Alias> Aliases;

        public Character()
        {
            Aliases = new List<Alias>();
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
        public async Task<List<Alias>> GetAliases() => await Alias.FetchAllByChar(this);

        public async Task SaveAll()
        {
            await UpdateAsync(this); // Update character
            await Appearance.UpdateAsync(Appearance); // Update Appearance
            foreach (var i in Aliases)
            {
                if (await Alias.Exist(i.CharID, i.AliasedID)) await Alias.UpdateAlias(i);
                else await Alias.CreateAsync(i.CharID, i.AliasedID, i.AliasName, i.AliasDesc); 
            }
        }
    }
}
