using GTANetworkAPI;

namespace RPServer.Models.CharacterHelpers
{
    internal class SkinCustomization
    {
        private bool _isMale;
        private PedHash _skinModel;

        public PedHash SkinModel
        {
            set
            {
                if (value == PedHash.FreemodeMale01) _isMale = true;
                if (value == PedHash.FreemodeFemale01) _isMale = false;
                _skinModel = value;
            }
            get => _skinModel;
        }
        public bool IsMale
        {
            set
            {
                if (SkinModel == PedHash.FreemodeMale01) _isMale = true;
                else if (SkinModel == PedHash.FreemodeFemale01) _isMale = false;
                else _isMale = value;
            }
            get
            {
                if (SkinModel == PedHash.FreemodeMale01)
                    return true;
                if (SkinModel == PedHash.FreemodeFemale01)
                    return false;
                return _isMale;
            }
        }
        public CustomHeadBlend CustomHeadBlend { get; set; }
        public CustomHeadOverlay CustomHeadOverlay { get; set; }
        public CustomFaceFeature CustomFaceFeatures { get; set; }
        public byte EyeColor { set; get; } // 0 .. 31
        public byte HairColor { set; get; } // 0 .. 63
        public byte HighlightColor { set; get; } // 0 .. 63

        public byte HairStyle { get; set; }
        public byte HairStyleTexture { get; set; }

        public SkinCustomization()
        {
            SkinModel = PedHash.FreemodeMale01;
            CustomHeadBlend = new CustomHeadBlend(0, 0, 0, 0);
            CustomHeadOverlay = new CustomHeadOverlay();
            CustomFaceFeatures = new CustomFaceFeature();
        }

        public void ApplyAll(Client client)
        {
            client.SetCustomization(IsMale, CustomHeadBlend.Get(), EyeColor, HairColor, HighlightColor, CustomFaceFeatures.Get(), CustomHeadOverlay.Get(), new Decoration[0]);
            client.SetClothes((int)Components.Hair, HairStyle, HairStyleTexture);
        }

    }

    // TODO: This will need to be moved to a component class wrapping SetClothes eventually
    internal enum Components
    {
        Face = 0,
        Mask = 1,
        Hair = 2,
        Torso = 3,
        Legs = 4,
        BagsParachutes =5,
        Shoes = 6,
        Accessories = 7,
        Undershirts = 8,
        BodyArmor = 9,
        Decals = 10,
        Tops = 11
    }
}
