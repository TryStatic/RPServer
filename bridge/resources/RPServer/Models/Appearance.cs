using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using GTANetworkAPI;

namespace RPServer.Models
{
    [Table("appearances")]
    internal class Appearance : Model<Appearance>
    {
        private bool _isMale;
        private PedHash _skinModel;

        public int CharacterID { set; get; }

        #region HEADBLEND_DATA
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
        public byte ShapeFirstID { get; set; }
        public byte ShapeSecondID { get; set; }
        public float ShapeMix { get; set; }
        public float SkinMix { get; set; }

        private const byte ShapeThirdID = 0;
        private const byte SkinThirdID = 0;
        private const float ThirdMix = 0.0f;
        private const bool IsParent = false;
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
        public byte HairColor { set; get; } // 0 .. 63
        public byte HighlightColor { set; get; } // 0 .. 63
        public byte HairStyleTexture { get; set; }
        public byte EyeColor { set; get; } // 0 .. 31

        #endregion

        public Appearance()
        {

        }

        /// <summary>
        /// Use to create new appearance for a character
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="charID"></param>
        public Appearance(PedHash ped, int charID)
        {
            SkinModel = ped;
            CharacterID = charID;
        }

        public void Apply(Client client)
        {
            if (SkinModel != PedHash.FreemodeMale01 && SkinModel != PedHash.FreemodeFemale01) return;

            client.SetCustomization(IsMale, GetHeadBlend(), EyeColor, HairColor, HighlightColor, GetFaceFeatures(), GetHeadOverlay(), new Decoration[0]);
            client.SetClothes((int)Components.Hair, HairStyle, HairStyleTexture);
        }

        public HeadBlend GetHeadBlend()
        {
            var blend = new HeadBlend()
            {
                ShapeFirst = ShapeFirstID,
                ShapeMix = ShapeMix,
                ShapeSecond = ShapeSecondID,
                ShapeThird = ShapeThirdID,
                SkinFirst = ShapeFirstID,
                SkinMix = SkinMix,
                SkinSecond = ShapeSecondID,
                SkinThird = SkinThirdID,
                ThirdMix = ThirdMix
            };
            return blend;
        }
        public Dictionary<int, HeadOverlay> GetHeadOverlay()
        {
            var dict = new Dictionary<int, HeadOverlay>
            {
                {
                    (int) HeadOverlayID.Blemishes,
                    new HeadOverlay()
                    {
                        Index = Blemishes,
                        Opacity = BlemishesOpacity,
                        Color = BlemishesColor,
                        SecondaryColor = BlemishesSecColor
                    }
                },
                {
                    (int) HeadOverlayID.FacialHair,
                    new HeadOverlay()
                    {
                        Index = FacialHair,
                        Opacity = FacialHairOpacity,
                        Color = FacialHairColor,
                        SecondaryColor = FacialHairSecColor
                    }
                },
                {
                    (int) HeadOverlayID.Eyebrows,
                    new HeadOverlay()
                    {
                        Index = Eyebrows,
                        Opacity = EyebrowsOpacity,
                        Color = EyebrowsColor,
                        SecondaryColor = EyebrowsSecColor
                    }
                },
                {
                    (int) HeadOverlayID.Ageing,
                    new HeadOverlay()
                    {
                        Index = Ageing,
                        Opacity = AgeingOpacity,
                        Color = AgeingColor,
                        SecondaryColor = AgeingSecColor
                    }
                },
                {
                    (int) HeadOverlayID.Makeup,
                    new HeadOverlay()
                    {
                        Index = Makeup,
                        Opacity = MakeupOpacity,
                        Color = MakeupColor,
                        SecondaryColor = MakeupSecColor
                    }
                },
                {
                    (int) HeadOverlayID.Blush,
                    new HeadOverlay()
                    {
                        Index = Blush,
                        Opacity = BlushOpacity,
                        Color = BlushColor,
                        SecondaryColor = BlushSecColor
                    }
                },
                {
                    (int) HeadOverlayID.Complexion,
                    new HeadOverlay()
                    {
                        Index = Complexion,
                        Opacity = ComplexionOpacity,
                        Color = ComplexionColor,
                        SecondaryColor = ComplexionSecColor
                    }
                },
                {
                    (int) HeadOverlayID.SunDamage,
                    new HeadOverlay()
                    {
                        Index = SunDamage,
                        Opacity = SunDamageOpacity,
                        Color = SunDamageColor,
                        SecondaryColor = SunDamageSecColor
                    }
                },
                {
                    (int) HeadOverlayID.Lipstick,
                    new HeadOverlay()
                    {
                        Index = Lipstick,
                        Opacity = LipstickOpacity,
                        Color = LipstickColor,
                        SecondaryColor = LipstickSecColor
                    }
                },
                {
                    (int) HeadOverlayID.Freckles,
                    new HeadOverlay()
                    {
                        Index = Freckles,
                        Opacity = FrecklesOpacity,
                        Color = FrecklesColor,
                        SecondaryColor = FrecklesSecColor
                    }
                },
                {
                    (int) HeadOverlayID.ChestHair,
                    new HeadOverlay()
                    {
                        Index = ChestHair,
                        Opacity = ChestHairOpacity,
                        Color = ChestHairColor,
                        SecondaryColor = ChestHairSecColor
                    }
                },
                {
                    (int) HeadOverlayID.BodyBlemishes,
                    new HeadOverlay()
                    {
                        Index = BodyBlemishes,
                        Opacity = BodyBlemishesOpacity,
                        Color = BodyBlemishesColor,
                        SecondaryColor = BodyBlemishesSecColor
                    }
                },
                {
                    (int) HeadOverlayID.AdditionalBodyBlemishes,
                    new HeadOverlay()
                    {
                        Index = AdditionalBodyBlemishes,
                        Opacity = AdditionalBodyBlemishesOpacity,
                        Color = AdditionalBodyBlemishesColor,
                        SecondaryColor = AdditionalBodyBlemishesSecColor
                    }
                }

            };
            return dict;
        }
        public float[] GetFaceFeatures()
        {
            var arr = new float[]
            {
                NoseWidth,
                NoseHeight,
                NoseLength,
                NoseBridge,
                NoseTip,
                NoseBridgeShift,
                BrowHeight,
                BrowWidth,
                CheekboneHeight,
                CheekboneWidth,
                CheeksWidth,
                Eyes,
                Lips,
                JawWidth,
                JawHeight,
                ChinLength,
                ChinPosition,
                ChinWidth,
                ChinShape,
                NeckWidth
            };

            return arr;
        }
    }

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
    internal enum HeadOverlayID
    {
        Blemishes = 0,
        FacialHair = 1,
        Eyebrows = 2,
        Ageing = 3,
        Makeup = 4,
        Blush = 5,
        Complexion = 6,
        SunDamage = 7,
        Lipstick = 8,
        Freckles = 9,
        ChestHair = 10,
        BodyBlemishes = 11,
        AdditionalBodyBlemishes = 12
    }
    internal enum FeatureID
    {
        NoseWidth = 0,
        NoseHeight = 1,
        NoseLength = 2,
        NoseBridge = 3,
        NoseTip = 4,
        NoseBridgeShift = 5,
        BrowHeight = 6,
        BrowWidth = 7,
        CheekboneHeight = 8,
        CheekboneWidth = 9,
        CheeksWidth = 10,
        Eyes = 11,
        Lips = 12,
        JawWidth = 13,
        JawHeight = 14,
        ChinLength = 15,
        ChinPosition = 16,
        ChinWidth = 17,
        ChinShape = 18,
        NeckWidth = 19
    }
}
