using Dapper.Contrib.Extensions;

namespace RPServer.Models
{
    [Table("vehicles")]
    internal class Vehicle : Model<Vehicle>
    {
        public int OwnerID { get; set; }

        public Vehicle()
        {
        }

        public Vehicle(int ownerID)
        {
            OwnerID = ownerID;
        }

    }
}
