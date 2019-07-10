using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using RPServer.Game;
using RPServer.Util;

namespace RPServer.Controllers
{
    public static class GoogleAuthenticator
    {
        private const int IntervalLength = 30;
        private const int PinLength = 6;
        private static readonly int PinModulo = (int)Math.Pow(10, PinLength);

        /// <summary>
        ///   Number of intervals that have elapsed.
        /// </summary>
        private static long CurrentInterval
        {
            get
            {
                var elapsedSeconds = (long)Math.Floor((DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds);

                return elapsedSeconds / IntervalLength;
            }
        }

        /// <summary>
        ///   Generates a QR code bitmap for provisioning.
        /// </summary>
        public static string GetGQCodeImageLink(string username, byte[] key, int width, int height)
        {
            var keyString = Encoder.Base32Encode(key);
            var provisionUrl = Encoder.UrlEncode($"otpauth://totp/{username}?secret={keyString}&issuer={Initialization.SERVER_NAME}");

            var chartUrl = $"https://chart.apis.google.com/chart?cht=qr&chs={width}x{height}&chl={provisionUrl}";
            return chartUrl;
            /*using (var client = new WebClient())
            {
                return client.DownloadData(chartUrl);
            }*/
        }

        public static byte[] GenerateTwoFactorGASharedKey()
        {
            return RandomGenerator.GetInstance().GenerateRandomBytes(50);
        }

        /// <summary>
        ///   Generates a pin for the given key.
        /// </summary>
        public static string GeneratePin(byte[] key)
        {
            return GeneratePin(key, CurrentInterval);
        }

        /// <summary>
        ///   Generates a pin by hashing a key and counter.
        /// </summary>
        static string GeneratePin(byte[] key, long counter)
        {
            const int sizeOfInt32 = 4;

            var counterBytes = BitConverter.GetBytes(counter);

            if (BitConverter.IsLittleEndian)
            {
                //spec requires bytes in big-endian order
                Array.Reverse(counterBytes);
            }

            var hash = new HMACSHA1(key).ComputeHash(counterBytes);
            var offset = hash[hash.Length - 1] & 0xF;

            var selectedBytes = new byte[sizeOfInt32];
            Buffer.BlockCopy(hash, offset, selectedBytes, 0, sizeOfInt32);

            if (BitConverter.IsLittleEndian)
            {
                //spec interprets bytes in big-endian order
                Array.Reverse(selectedBytes);
            }

            var selectedInteger = BitConverter.ToInt32(selectedBytes, 0);

            //remove the most significant bit for interoperability per spec
            var truncatedHash = selectedInteger & 0x7FFFFFFF;

            //generate number of digits for given pin length
            var pin = truncatedHash % PinModulo;

            return pin.ToString(CultureInfo.InvariantCulture).PadLeft(PinLength, '0');
        }
        #region Nested type: Encoder

        private static class Encoder
        {
            /// <summary>
            ///   Url Encoding (with upper-case hexadecimal per OATH specification)
            /// </summary>
            public static string UrlEncode(string value)
            {
                const string urlEncodeAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

                var builder = new StringBuilder();

                foreach (var symbol in value)
                {
                    if (urlEncodeAlphabet.IndexOf(symbol) != -1)
                    {
                        builder.Append(symbol);
                    }
                    else
                    {
                        builder.Append('%');
                        builder.Append(((int)symbol).ToString("X2"));
                    }
                }
                return builder.ToString();
            }

            /// <summary>
            ///   Base-32 Encoding
            /// </summary>
            public static string Base32Encode(byte[] data)
            {
                const int inByteSize = 8;
                const int outByteSize = 5;
                const string base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

                int i = 0, index = 0;
                var builder = new StringBuilder((data.Length + 7) * inByteSize / outByteSize);

                while (i < data.Length)
                {
                    int currentByte = data[i];
                    int digit;

                    //Is the current digit going to span a byte boundary?
                    if (index > (inByteSize - outByteSize))
                    {
                        int nextByte;

                        if ((i + 1) < data.Length)
                        {
                            nextByte = data[i + 1];
                        }
                        else
                        {
                            nextByte = 0;
                        }

                        digit = currentByte & (0xFF >> index);
                        index = (index + outByteSize) % inByteSize;
                        digit <<= index;
                        digit |= nextByte >> (inByteSize - index);
                        i++;
                    }
                    else
                    {
                        digit = (currentByte >> (inByteSize - (index + outByteSize))) & 0x1F;
                        index = (index + outByteSize) % inByteSize;

                        if (index == 0)
                        {
                            i++;
                        }
                    }

                    builder.Append(base32Alphabet[digit]);
                }

                return builder.ToString();
            }
        }

        #endregion
    }
}