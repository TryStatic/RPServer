using GTANetworkAPI;

namespace RPServer.Models.CharacterHelpers
{
    internal class CustomHeadBlend
    {
        public byte ShapeFirstID { get; set; }
        public byte ShapeSecondID { get; set; }

        public float ShapeMix { get; private set; }
        public float SkinMix { get; private set; }

        #region DEFAULT_BLEND_DATA_VALUES
        public byte SkinFirstID => ShapeFirstID;
        public byte SkinSecondID => ShapeSecondID;
        private static byte ShapeThirdID => 0;
        private static byte SkinThirdID => 0;
        private static float ThirdMix => 0.0f;
        private static bool IsParent => false;
        #endregion

        public CustomHeadBlend(byte shapeFirst, byte shapeSecond, float shapeMix, float skinMix)
        {
            ShapeFirstID = shapeFirst;
            ShapeSecondID = shapeSecond;
            ShapeMix = shapeMix;
            SkinMix = skinMix;

        }

        public HeadBlend Get()
        {
            var blend = new HeadBlend()
            {
                ShapeFirst = ShapeFirstID,
                ShapeMix = ShapeMix,
                ShapeSecond = ShapeSecondID,
                ShapeThird = ShapeThirdID,
                SkinFirst = SkinFirstID,
                SkinMix = SkinMix,
                SkinSecond = SkinSecondID,
                SkinThird = SkinThirdID,
                ThirdMix = ThirdMix
            };
            return blend;
        }

        public void Apply(Client client)
        {
            client.HeadBlend = new HeadBlend()
            {
                ShapeFirst = ShapeFirstID,
                ShapeMix = ShapeMix,
                ShapeSecond = ShapeSecondID,
                ShapeThird = ShapeThirdID,
                SkinFirst = SkinFirstID,
                SkinMix = SkinMix,
                SkinSecond = SkinSecondID,
                SkinThird = SkinThirdID,
                ThirdMix = ThirdMix
            };
        }
    }
}