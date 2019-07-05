using GTANetworkAPI;

namespace RPServer.Models.CharacterHelpers
{
    internal class SkinCustomization
    {
        private bool _isMale;

        public HeadBlend HeadBlend { get; set; }
        public FaceFeature FaceFeatures { get; set; }
        public HeadOverlay HeadOverlay { get; set; }
        public PedHash SkinModel { set; get; }
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

        public byte EyeColor { set; get; }
        public byte HairColor { set; get; }
        public byte HighlightColor { set; get; }

        public void ApplyAll(Client client)
        {
            client.SetCustomization(IsMale, HeadBlend.Get(), EyeColor, HairColor, HighlightColor, FaceFeatures.Get(), HeadOverlay.Get(), new Decoration[0]);
        }

    }
}
