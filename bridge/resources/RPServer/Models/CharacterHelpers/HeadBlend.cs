using GTANetworkAPI;

namespace RPServer.Models.CharacterHelpers
{
    internal class HeadBlend
    {
        public byte ShapeFirstID { get; }
        public byte ShapeSecondID { get; }

        public byte SkinFirstID => ShapeFirstID;
        public byte SkinSecondID => ShapeSecondID;

        public float ShapeMix { get; private set; }
        public float SkinMix { get; private set; }

        public byte ShapeThirdID => 0;
        public byte SkinThirdID => 0;
        public float ThirdMix => 0.0f;
        public bool IsParent => false;

        public HeadBlend(Client client, byte shapeFirst, byte shapeSecond, float shapeMix, float skinMix)
        {
            ShapeFirstID = shapeFirst;
            ShapeSecondID = shapeSecond;
            ShapeMix = shapeMix;
            SkinMix = skinMix;
            Apply(client);

        }

        public void Update(Client client, float newShapeMix, float newSkinMix)
        {
            ShapeMix = newShapeMix;
            SkinMix = newSkinMix;
            client.UpdateHeadBlend(ShapeMix, SkinMix, ThirdMix);
        }

        public void Apply(Client client)
        {
            client.HeadBlend = new GTANetworkAPI.HeadBlend()
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