using System;
using System.Security.Cryptography;
using System.Text;

namespace Unipluss.Sign.StorageService.Server.Code
{
    public static class RandomStringGenerator
    {
        private static readonly RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();

        public static string GetRandomString(int length, params char[] chars)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                byte[] intBytes = new byte[4];
                Rand.GetBytes(intBytes);
                uint randomInt = BitConverter.ToUInt32(intBytes, 0);
                s.Append(chars[randomInt % chars.Length]);
            }
            return s.ToString();

        }

        public static string GetRandomString(int length)
        {
            return GetRandomString(length, "abcdefghijklmnopqrstuvwzxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_-".ToCharArray());
        }

       
    }
}