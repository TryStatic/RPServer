using GTANetworkAPI;

namespace RPServer.Models.Helpers
{
    internal class FaceFeatures
    {
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

        public void Apply(Client client)
        {
            client.SetFaceFeature((int)Slots.NoseWidth, NoseWidth);
            client.SetFaceFeature((int)Slots.NoseHeight, NoseHeight);
            client.SetFaceFeature((int)Slots.NoseLength, NoseLength);
            client.SetFaceFeature((int)Slots.NoseBridge, NoseBridge);
            client.SetFaceFeature((int)Slots.NoseTip, NoseTip);
            client.SetFaceFeature((int)Slots.NoseBridgeShift, NoseBridgeShift);
            client.SetFaceFeature((int)Slots.BrowHeight, BrowHeight);
            client.SetFaceFeature((int)Slots.BrowWidth, BrowWidth);
            client.SetFaceFeature((int)Slots.CheekboneHeight, CheekboneHeight);
            client.SetFaceFeature((int)Slots.CheekboneWidth, CheekboneWidth);
            client.SetFaceFeature((int)Slots.CheeksWidth, CheeksWidth);
            client.SetFaceFeature((int)Slots.Eyes, Eyes);
            client.SetFaceFeature((int)Slots.Lips, Lips);
            client.SetFaceFeature((int)Slots.JawWidth, JawWidth);
            client.SetFaceFeature((int)Slots.JawHeight, JawHeight);
            client.SetFaceFeature((int)Slots.ChinLength, ChinLength);
            client.SetFaceFeature((int)Slots.ChinPosition, ChinPosition);
            client.SetFaceFeature((int)Slots.ChinWidth, ChinWidth);
            client.SetFaceFeature((int)Slots.ChinShape, ChinShape);
            client.SetFaceFeature((int)Slots.NeckWidth, NeckWidth);
        }
    }

    internal enum Slots
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