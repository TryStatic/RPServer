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
        public async Task<Appearance> GetAppearance()
        {
            var app =  await Appearance.ReadByKeyAsync(() => new Appearance().CharacterID, this.ID);
            return app.FirstOrDefault();
        }
        public async Task<HashSet<Alias>> GetAliases() => await Alias.FetchAllByChar(this);
        public async Task<HashSet<Vehicle>> GetVehicles() => (await Vehicle.ReadByKeyAsync(() => new Vehicle().OwnerID, ID)).ToHashSet();

        public async Task SaveAll()
        {
            await UpdateAsync(this); // Update character
            await Appearance.UpdateAsync(Appearance); // Update Appearance
            await UpdateAlises();
            await UpdateVehicles();
        }
        public async Task FetchAll()
        {
            Appearance = await GetAppearance();
            Aliases = await GetAliases();
            Vehicles = await GetVehicles();
        }

        private async Task UpdateVehicles()
        {
            var dbRecsEnumerable = await Vehicle.ReadByKeyAsync(() => new Vehicle().OwnerID, ID);
            var dbRecords = dbRecsEnumerable.ToHashSet();
            foreach (var dbRec in dbRecords)
            {
                if (Vehicles.Contains(dbRec))
                {
                    await Vehicle.UpdateAsync(dbRec);
                    Vehicles.Remove(dbRec);
                }
                else
                {
                    await Vehicle.DeleteAsync(dbRec);
                }
            }

            foreach (var i in Vehicles)
            {
                await Vehicle.CreateAsync(new Vehicle(i.OwnerID));
            }
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
