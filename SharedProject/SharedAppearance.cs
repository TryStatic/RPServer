namespace Shared
{
    public class SharedAppearance
    {
        private bool _isMale;
        private uint _skinModel;

        public string FirstName { get; set; }
        public string LastName { get; set; }

        #region HEADBLEND_DATA
        public uint SkinModel
        {
            set
            {
                if (value == 2627665880) _isMale = true;
                if (value == 1885233650) _isMale = false;
                _skinModel = value;
            }
            get => _skinModel;
        }
        public bool IsMale
        {
            set
            {
                if (SkinModel == 2627665880) _isMale = true;
                else if (SkinModel == 1885233650) _isMale = false;
                else _isMale = value;
            }
            get
            {
                if (SkinModel == 2627665880)
                    return true;
                if (SkinModel == 1885233650)
                    return false;
                return _isMale;
            }
        }
        public byte ShapeFirstID { get; set; }
        public byte ShapeSecondID { get; set; }
        public float ShapeMix { get; set; }
        public float SkinMix { get; set; }
        #endregion

        #region HEAD_OVERLAY
        public byte Blemishes { set; get; }
        public byte FacialHair { set; get; }
        public byte Eyebrows { set; get; }
        public byte Ageing { set; get; }
        public byte Makeup { set; get; }
        public byte Blush { set; get; }
        public byte Complexion { set; get; }
        public byte SunDamage { set; get; }
        public byte Lipstick { set; get; }
        public byte Freckles { set; get; }
        public byte ChestHair { set; get; }
        public byte BodyBlemishes { set; get; }
        public byte AdditionalBodyBlemishes { set; get; }

        public float BlemishesOpacity { set; get; }
        public float FacialHairOpacity { set; get; }
        public float EyebrowsOpacity { set; get; }
        public float AgeingOpacity { set; get; }
        public float MakeupOpacity { set; get; }
        public float BlushOpacity { set; get; }
        public float ComplexionOpacity { set; get; }
        public float SunDamageOpacity { set; get; }
        public float LipstickOpacity { set; get; }
        public float FrecklesOpacity { set; get; }
        public float ChestHairOpacity { set; get; }
        public float BodyBlemishesOpacity { set; get; }
        public float AdditionalBodyBlemishesOpacity { set; get; }

        public byte BlemishesColor { set; get; }
        public byte FacialHairColor { set; get; }
        public byte EyebrowsColor { set; get; }
        public byte AgeingColor { set; get; }
        public byte MakeupColor { set; get; }
        public byte BlushColor { set; get; }
        public byte ComplexionColor { set; get; }
        public byte SunDamageColor { set; get; }
        public byte LipstickColor { set; get; }
        public byte FrecklesColor { set; get; }
        public byte ChestHairColor { set; get; }
        public byte BodyBlemishesColor { set; get; }
        public byte AdditionalBodyBlemishesColor { set; get; }

        public byte BlemishesSecColor { set; get; }
        public byte FacialHairSecColor { set; get; }
        public byte EyebrowsSecColor { set; get; }
        public byte AgeingSecColor { set; get; }
        public byte MakeupSecColor { set; get; }
        public byte BlushSecColor { set; get; }
        public byte ComplexionSecColor { set; get; }
        public byte SunDamageSecColor { set; get; }
        public byte LipstickSecColor { set; get; }
        public byte FrecklesSecColor { set; get; }
        public byte ChestHairSecColor { set; get; }
        public byte BodyBlemishesSecColor { set; get; }
        public byte AdditionalBodyBlemishesSecColor { set; get; }
        #endregion

        #region FACE_FEATURES
        public float NoseWidth { get; set; }
        public float NoseHeight { get; set; }
        public float NoseLength { get; set; }
        public float NoseBridge { get; set; }
        public float NoseTip { get; set; }
        public float NoseBridgeShift { get; set; }
        public float BrowHeight { get; set; }
        public float BrowWidth { get; set; }
        public float CheekboneHeight { get; set; }
        public float CheekboneWidth { get; set; }
        public float CheeksWidth { get; set; }
        public float Eyes { get; set; }
        public float Lips { get; set; }
        public float JawWidth { get; set; }
        public float JawHeight { get; set; }
        public float ChinLength { get; set; }
        public float ChinPosition { get; set; }
        public float ChinWidth { get; set; }
        public float ChinShape { get; set; }
        public float NeckWidth { get; set; }
        #endregion

        #region EXTRA
        public byte HairStyle { get; set; }
        public byte HairStyleTexture { get; set; }

        public byte EyeColor { set; get; } // 0 .. 31
        public byte HairColor { set; get; } // 0 .. 63
        public byte HighlightColor { set; get; } // 0 .. 63
        #endregion
    }
}
