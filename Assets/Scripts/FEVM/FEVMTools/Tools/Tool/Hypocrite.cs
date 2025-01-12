using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FEVM.Hypocrite
{
    public class Hypocrite
    {
        public string secret = "Alan_FEVM>>>TheMaster";

        public static string ToHypocrite(string plainText, string mm)
        {
// #if UNITY_EDITOR
//         return plainText;
// #else
            //分组加密算法
            SymmetricAlgorithm des = Rijndael.Create();
            des.Padding = PaddingMode.ISO10126;
            des.Mode = CipherMode.CBC;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText); //得到需要加密的字节数组
            des.KeySize = 128;
            des.BlockSize = 128;
            //设置密钥及密钥向量
            byte[] _key1 = Encoding.UTF8.GetBytes(mm);
            des.Key = _key1;
            des.IV = _key1;
            byte[] cipherBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cipherBytes = ms.ToArray(); //得到加密后的字节数组
                    cs.Close();
                    ms.Close();
                }
            }

            return Convert.ToBase64String(cipherBytes);
//#endif
        }


        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="content">密文字符串</param>
        /// <returns>明文</returns>
        public static string FromHypocrite(string content, string mm)
        {
// #if UNITY_EDITOR
//         return encryptStr;
//         #else
//         
            string decrypt = "";
            Rijndael aes = Rijndael.Create();
            try
            {
                byte[] bKey = Encoding.UTF8.GetBytes(mm);
                byte[] bIV = Encoding.UTF8.GetBytes(mm);
                byte[] byteArray = Convert.FromBase64String(content);

                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.ISO10126;
                aes.Mode = CipherMode.CBC;

                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream =
                           new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(mStream.ToArray());
                    }
                }
            }
            catch
            {
            }

            aes.Clear();

            return decrypt;
//#endif
        }
    }
}