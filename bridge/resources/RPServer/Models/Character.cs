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
        public HashSet<Alias> Aliases;
        public HashSet<Vehicle> Vehicles;

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

        public async Task SaveAllData()
        {
            // This Character Instance
            await UpdateAsync(this);
            // One to One Relationships
            await Appearance.UpdateAsync();
            // One to Many Relationships (data must be HashSet<T> where T a Model descendant)
            await Vehicle.UpdateAllByKeyAsync(() => new Vehicle().OwnerID, ID, Vehicles);
            // Other
            await Alias.UpdateAllByChar(Aliases, this);
        }
        public async Task ReadAllData()
        {
            Appearance = (await Appearance.ReadByKeyAsync(() => new Appearance().CharacterID, this.ID)).FirstOrDefault();
            Aliases = await Alias.ReadAllByChar(this);
            Vehicles = (await Vehicle.ReadByKeyAsync(() => new Vehicle().OwnerID, ID)).ToHashSet();
        }
    }
}
