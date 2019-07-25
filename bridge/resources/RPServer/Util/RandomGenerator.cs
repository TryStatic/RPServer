using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace RPServer.Util
{
    internal sealed class RandomGenerator
    {
        private static RandomGenerator _randomGenerator;
        private readonly RNGCryptoServiceProvider _csp;
        private Stack<int> UniqueRandomPool;
        private RandomGenerator()
        {
            _csp = new RNGCryptoServiceProvider();
            InitUniqueRandPool();
        }

        /// <summary>
        /// After ~9000 generations it re-initializes which means you can get dupes
        /// </summary>
        private void InitUniqueRandPool()
        {
            var nums = Enumerable.Range(1000, 9999).ToArray();
            for (var i = 0; i < nums.Length; ++i)
            {
                var randomIndex = Next(0, nums.Length);
                var temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }
            UniqueRandomPool = new Stack<int>(nums);
        }

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

        public int UniqueNext()
        {
            if(UniqueRandomPool.Count == 0) InitUniqueRandPool();
            return UniqueRandomPool.Pop();
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