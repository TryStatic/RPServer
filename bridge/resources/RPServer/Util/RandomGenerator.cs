using System;
using System.Security.Cryptography;

namespace RPServer.Util
{
    internal sealed class RandomGenerator
    {
        private static RandomGenerator _randomGenerator;
        private readonly RNGCryptoServiceProvider _csp;

        private RandomGenerator() => _csp = new RNGCryptoServiceProvider();

        public int Next(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
                throw new ArgumentOutOfRangeException();

            long diff = (long)maxExclusiveValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }
        private uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }
        public byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            _csp.GetBytes(buffer);
            return buffer;
        }

        public static RandomGenerator GetInstance() => _randomGenerator ?? (_randomGenerator = new RandomGenerator());
    }
}