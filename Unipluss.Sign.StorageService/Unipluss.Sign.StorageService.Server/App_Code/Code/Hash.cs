
    using System;
    using System.Security.Cryptography;
    using System.Text;

    namespace Unipluss.Sign.StorageService.Server.Code
    {
        public enum HashType : int
        {
            MD5,
            SHA1,
            SHA256,
            SHA512,
            BCRYPT,
            SHA384,
        }

        public class Hash
        {
            public Hash() { }



            public static string GetHash(string text, string key, HashType hashType, Encoding encoding = null)
            {
                string hashString;
                if (encoding == null)
                    encoding = new UnicodeEncoding();
                switch (hashType)
                {
                    case HashType.MD5:
                        hashString = GetMD5(text, key, encoding);
                        break;
                    case HashType.SHA1:
                        hashString = GetSHA1(text, key, encoding);
                        break;
                    case HashType.SHA256:
                        hashString = GetSHA256(text, key, encoding);
                        break;
                    case HashType.SHA512:
                        hashString = GetSHA512(text, key, encoding);
                        break;
                    default:
                        hashString = "Invalid Hash Type";
                        break;
                }
                return hashString;
            }

            public static string GetHash(string text, HashType hashType)
            {
                string hashString;
                switch (hashType)
                {
                    case HashType.MD5:
                        hashString = GetMD5(text);
                        break;
                    case HashType.SHA1:
                        hashString = GetSHA1(text);
                        break;
                    case HashType.SHA256:
                        hashString = GetSHA256(text);
                        break;
                    case HashType.SHA512:
                        hashString = GetSHA512(text);
                        break;
                    default:
                        hashString = "Invalid Hash Type";
                        break;
                }
                return hashString;
            }

            public static bool CheckHash(string original, string hashString, HashType hashType)
            {

                if (hashType == HashType.BCRYPT)
                {
                    //return BCrypt.Net.BCrypt.Verify(original, hashString);
                    return false;

                }
                else
                {
                    string originalHash = GetHash(original, hashType);
                    return (originalHash == hashString);
                }

            }

            public static string GenerateSalt()
            {
                byte[] buf = new byte[16];
                (new RNGCryptoServiceProvider()).GetBytes(buf);
                return Convert.ToBase64String(buf);
            }


            public static string GetSHA512(byte[] content)
            {

                return ByteToString(new SHA512Managed().ComputeHash(content));
            }

            public static string GetSha256(byte[] content)
            {
                return ByteToString(new SHA256Managed().ComputeHash(content));
            }

            public static string GetSha256Base64(byte[] content)
            {
                return Convert.ToBase64String(new SHA256Managed().ComputeHash(content));
            }

            public static string GetSHA1(byte[] content)
            {

                return ByteToString(new SHA1Managed().ComputeHash(content));
            }

            //public static string GetHash(string text,int workFactor){            
            //    return BCrypt.Net.BCrypt.HashString(text, workFactor);                       
            //}



            private static string GetMD5(string text)
            {
                UnicodeEncoding encoding = new UnicodeEncoding();
                byte[] hashValue;
                byte[] message = encoding.GetBytes(text);

                MD5 hashString = new MD5CryptoServiceProvider();

                hashValue = hashString.ComputeHash(message);
                return ByteToString(hashValue);

            }

            private static string GetMD5(string text, string key, Encoding encoding)
            {
                byte[] keyByte = encoding.GetBytes(key);
                HMACMD5 hmacmd5 = new HMACMD5(keyByte);

                byte[] messageBytes = encoding.GetBytes(text);
                byte[] hashmessage = hmacmd5.ComputeHash(messageBytes);
                return ByteToString(hashmessage);

            }

            private static string GetSHA1(string text)
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] hashValue;
                byte[] message = UE.GetBytes(text);

                SHA1Managed hashString = new SHA1Managed();

                hashValue = hashString.ComputeHash(message);
                return ByteToString(hashValue);
            }

            private static string GetSHA1(string text, string key, Encoding encoding)
            {
                byte[] keyByte = encoding.GetBytes(key);
                HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);

                byte[] messageBytes = encoding.GetBytes(text);
                byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
                return ByteToString(hashmessage);

            }

            private static string GetSHA256(string text)
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] hashValue;
                byte[] message = UE.GetBytes(text);

                SHA256Managed hashString = new SHA256Managed();

                hashValue = hashString.ComputeHash(message);
                return ByteToString(hashValue);
            }

            private static string GetSHA256(string text, string key, Encoding encoding)
            {

                byte[] keyByte = encoding.GetBytes(key);
                HMACSHA256 hmacsha256 = new HMACSHA256(keyByte);

                byte[] messageBytes = encoding.GetBytes(text);
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return ByteToString(hashmessage);

            }

            private static string GetSHA512(string text)
            {
                UnicodeEncoding encoding = new UnicodeEncoding();
                byte[] hashValue;
                byte[] message = encoding.GetBytes(text);

                SHA512Managed hashString = new SHA512Managed();

                hashValue = hashString.ComputeHash(message);
                return ByteToString(hashValue);
            }

            private static string GetSHA512(string text, string key, Encoding encoding)
            {

                byte[] keyByte = encoding.GetBytes(key);
                HMACSHA512 hmacsha512 = new HMACSHA512(keyByte);

                byte[] messageBytes = encoding.GetBytes(text);
                byte[] hashmessage = hmacsha512.ComputeHash(messageBytes);
                return ByteToString(hashmessage);

            }

            private static string ByteToString(byte[] buff)
            {
                string sbinary = "";

                for (int i = 0; i < buff.Length; i++)
                {
                    sbinary += buff[i].ToString("X2"); // hex format
                }
                return (sbinary);
            }
        
    }

}
