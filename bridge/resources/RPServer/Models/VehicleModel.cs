﻿using System.Configuration.Internal;
using Dapper.Contrib.Extensions;

namespace RPServer.Models
{
    [Table("vehicles")]
    internal class VehicleModel : Model<VehicleModel>
    {
        private string _plateText;
        public int OwnerID { get; set; }

        public uint Model { set; get; }
        public int PrimaryColor { set; get; }
        public int SecondaryColor { set; get; }
        public string PlateText
        {
            set => _plateText = value.Length <= 8 ? value : value.Remove(8); // Can't display more than 8 chars
            get => _plateText;
        }
        public short PlateStyle { set; get; }

        public VehicleModel()
        {
        }

        public VehicleModel(int ownerID)
        {
            OwnerID = ownerID;
        }
    }
}
