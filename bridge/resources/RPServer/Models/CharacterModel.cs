using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using RPServer.Util;

namespace RPServer.Models
{
    [Table("characters")]
    internal class CharacterModel : Model<CharacterModel>
    {
        public int CharOwnerID { set; get; }
        public string CharacterName { set; get; }

        public AppearanceModel Appearance;
        public HashSet<Alias> Aliases;
        public HashSet<VehicleModel> Vehicles;

        public CharacterModel()
        {
            
        }

        /// <summary>
        /// Use to create new character
        /// </summary>
        public static async Task CreateNewAsync(AccountModel charOwner, string newCharName)
        {
            var newChar = new CharacterModel()
            {
                CharOwnerID = charOwner.ID,
                CharacterName = newCharName
            };
            await newChar.CreateAsync();
        }
        public static async Task<List<CharacterModel>> FetchAllAsync(AccountModel account)
        {
            var result = await ReadByKeyAsync(() => new CharacterModel().CharOwnerID, account.ID);
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
            await VehicleModel.UpdateAllByKeyAsync(() => new VehicleModel().OwnerID, ID, Vehicles);
            // Other
            await Alias.UpdateAllByChar(Aliases, this);
        }
        public async Task ReadAllData()
        {
            Appearance = (await AppearanceModel.ReadByKeyAsync(() => new AppearanceModel().CharacterID, this.ID)).FirstOrDefault();
            Aliases = await Alias.ReadAllByChar(this);
            Vehicles = (await VehicleModel.ReadByKeyAsync(() => new VehicleModel().OwnerID, ID)).ToHashSet();
        }
    }
}
