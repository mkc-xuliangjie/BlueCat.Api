using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BuleCat.Common.Algorithms.Cryptography
{
    /// <summary>
    /// 3DES 加密/解密
    /// </summary>
    public static class DES3Crypto
    {
        /// <summary>
        /// 3DES CBC 模式加密。使用的 UTF-8 格式编码。
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="scrambledKey">对称秘钥</param>
        public static string EncryptCBC(string encryptString, byte[] iv, byte[] scrambledKey)
        {
            return EncryptCBC(encryptString, iv, scrambledKey, Encoding.UTF8);
        }

        /// <summary>
        /// 3DES CBC 模式加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="scrambledKey">对称秘钥</param>
        /// <param name="encoding">编码</param>
        /// <returns>加密后的字符串, 以 Base64 位输出</returns>
        public static string EncryptCBC(string encryptString, byte[] iv, byte[] scrambledKey, Encoding encoding)
        {
            var des = new TripleDESCryptoServiceProvider
            {
                IV = iv,
                Key = scrambledKey,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            using (var mStream = new MemoryStream())
            using (var cStream = new CryptoStream(mStream, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
                var data = encoding.GetBytes(encryptString);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();

                return Convert.ToBase64String(mStream.ToArray());
            }
        }

        /// <summary>
        /// 3DES CBC 模式解密。使用的 UTF-8 格式编码。
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="scrambledKey">对称秘钥</param>
        public static string DecryptCBC(string decryptString, byte[] iv, byte[] scrambledKey)
        {
            return DecryptCBC(decryptString, iv, scrambledKey, Encoding.UTF8);
        }

        /// <summary>
        /// 3DES CBC 模式解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="iv">初始化向量</param>
        /// <param name="scrambledKey">对称秘钥</param>
        /// <param name="encoding">编码</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptCBC(string decryptString, byte[] iv, byte[] scrambledKey, Encoding encoding)
        {
            var des = new TripleDESCryptoServiceProvider
            {
                IV = iv,
                Key = scrambledKey,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            using (var mStream = new MemoryStream())
            using (var cStream = new CryptoStream(mStream, des.CreateDecryptor(), CryptoStreamMode.Write))
            {
                var data = Convert.FromBase64String(decryptString);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();

                return encoding.GetString(mStream.ToArray());
            }
        }
    }
}
