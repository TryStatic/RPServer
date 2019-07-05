using GTANetworkAPI;

namespace RPServer.Models.CharacterHelpers
{
    internal class CustomFaceFeature
    {
        private readonly Feature[] _features;

        public CustomFaceFeature()
        {
            _features = new []
            {
                new Feature(FeatureIndex.NoseWidth, 0),
                new Feature(FeatureIndex.NoseHeight, 0),
                new Feature(FeatureIndex.NoseLength, 0),
                new Feature(FeatureIndex.NoseBridge, 0),
                new Feature(FeatureIndex.NoseTip, 0),
                new Feature(FeatureIndex.NoseBridgeShift, 0),
                new Feature(FeatureIndex.BrowHeight, 0),
                new Feature(FeatureIndex.BrowWidth, 0),
                new Feature(FeatureIndex.CheekboneHeight, 0),
                new Feature(FeatureIndex.CheekboneWidth, 0),
                new Feature(FeatureIndex.CheeksWidth, 0),
                new Feature(FeatureIndex.Eyes, 0),
                new Feature(FeatureIndex.Lips, 0),
                new Feature(FeatureIndex.JawWidth, 0),
                new Feature(FeatureIndex.JawHeight, 0),
                new Feature(FeatureIndex.ChinLength, 0),
                new Feature(FeatureIndex.ChinPosition, 0),
                new Feature(FeatureIndex.ChinWidth, 0),
                new Feature(FeatureIndex.ChinShape, 0),
                new Feature(FeatureIndex.NeckWidth, 0),
            };
        }

        public float[] Get()
        {
            var arr = new float[_features.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = _features[i].Value;
            }
            return arr;
        }

        public void SetFeature(Client client, FeatureIndex index, float value)
        {
            _features[(int)index].Value = value;
            client.SetFaceFeature((int)index, value);
        }

        public void ApplyAll(Client client)
        {
            foreach (var feature in _features)
            {
                client.SetFaceFeature(feature.Index, feature.Value);
            }
        }

        private struct Feature
        {
            public int Index { get; }
            public float Value { set; get; }

            internal Feature(FeatureIndex featureIndex, float value)
            {
                Index = (int)featureIndex;
                Value = value;
            }
        }
    }

    internal enum FeatureIndex
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