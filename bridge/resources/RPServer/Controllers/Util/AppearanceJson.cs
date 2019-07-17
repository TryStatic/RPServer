namespace RPServer.Controllers.Util
{
    public class AppearanceJson
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public bool isMale { get; set; }
        public int ShapeFirst { get; set; }
        public int ShapeSecond { get; set; }
        public int SkinSecond { get; set; }
        public float ShapeMix { get; set; }
        public float SkinMix { get; set; }
        public float[] FaceFeatures { get; set; }
        public float[][] Overlays { get; set; }
        public int Hairstyle { get; set; }
        public int HairColor { get; set; }
        public int HairHighlightColor { get; set; }
        public int HairStyleTexture { get; set; }
        public int EyeColor { get; set; }
    }
}