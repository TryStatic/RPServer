using Dapper.Contrib.Extensions;

namespace RPServer.Models
{
    [Table("vehicles")]
    internal class VehicleModel : Model<VehicleModel>
    {
        public int OwnerID { get; set; }

        public VehicleModel()
        {
        }

        public VehicleModel(int ownerID)
        {
            OwnerID = ownerID;
        }
    }
}
