using System;
using System.IO;
using System.Security.Cryptography;

namespace FEVM.Data
{
    internal class AesEncryption
    {
        // 生成随机的AES密钥和IV
        public static (byte[] Key, byte[] IV) GenerateAesKeyAndIv()
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.GenerateKey();
            aesAlg.GenerateIV();
            return (aesAlg.Key, aesAlg.IV);
        }

        // 固定的AES秘钥和初始化向量
        public static (byte[] Key, byte[] IV) ConstAesKeyAndIv()
        {
            // key
            const string KEY_HEX = "00112233445566778899AABBCCDDEEFF0011223344556677";
            // IV
            const string IV_HEX = "99887766554433221100FFEEDDCCBBAA";
            var m_key = HexToBytes(KEY_HEX);
            var m_IV = HexToBytes(IV_HEX);
            return (m_key, m_IV);
        }

        private static byte[] HexToBytes(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }


        // 加密字符串
        public static string EncryptString(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new();
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encrypted = msEncrypt.ToArray();
            }

            return Convert.ToBase64String(encrypted);
        }

        // 解密字符串
        public static string DecryptString(string cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new(fullCipher);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
    }
}
