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
                if (NAPI.Util.PedNameToModel("mp_m_freemode_01") == SkinModel) _isMale = true;
                else if (NAPI.Util.PedNameToModel("mp_f_freemode_01") == SkinModel) _isMale = false;
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
        public CustomFaceFeature CustomFaceFeatures { get; set; }
        public CustomHeadOverlay CustomHeadOverlay { get; set; }
        /// <summary>
        /// Between 0 and 31
        /// </summary>
        public byte EyeColor { set; get; }
        /// <summary>
        /// Between 0 and 63
        /// </summary>
        public byte HairColor { set; get; }
        /// <summary>
        /// Between 0 and 63
        /// </summary>
        public byte HighlightColor { set; get; }

        public byte HairStyle { get; set; }
        public byte HairStyleTexture { get; set; }


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
