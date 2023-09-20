using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IEM.Application.Utils
{
    public class RandomUtils
    {
        public static readonly string AlphanumericCharacters = "abcdefghijklmnopqrstuvwxyz1234567890";

        public static byte[] GenerateRandomBytes(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("length must not be negative", "length");
            }
            byte[] salt = new byte[length];
            RandomNumberGenerator.Create().GetBytes(salt);
            return salt;
        }

        public static string GetRandomAlphanumericString(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentException("length must not be negative", "length");
            }
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
            {
                throw new ArgumentException("length is too big", "length");
            }

            var characterArray = AlphanumericCharacters.Distinct().ToArray();
            var bytes = new byte[length * 8];
            var result = new char[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }
    }
}
