using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BuleCat.Common.Algorithms.Cryptography
{
    /// <summary>
    /// AES 加密/解密
    /// </summary>
    public static class AESCrypto
    {
        /// <summary>
        /// AES 加密。使用 UTF-8 的编码格式。
        /// 加密后的结果会经 Base64 编码输出。
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="key">Key</param>
        /// <param name="iv">向量</param>
        public static string Encrypt(string encryptString, byte[] key, byte[] iv)
        {
            return Encrypt(encryptString, key, iv, Encoding.UTF8);
        }

        /// <summary>
        /// AES 加密。
        /// 加密后的结果会经 Base64 编码输出。
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="key">Key</param>
        /// <param name="iv">向量</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static string Encrypt(string encryptString, byte[] key, byte[] iv, Encoding encoding)
        {
            var datas = encoding.GetBytes(encryptString);

            using (var aes = Aes.Create())
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                cryptoStream.Write(datas, 0, datas.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// AES 解密。使用 UTF-8 的编码格式。
        /// 会先将要解密的字符串经过 Base64 编码解码。
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="key">Key</param>
        /// <param name="iv">向量</param>
        public static string Decrypt(string decryptString, byte[] key, byte[] iv)
        {
            return Decrypt(decryptString, key, iv, Encoding.UTF8);
        }

        /// <summary>
        /// AES 解密。
        /// 会先将要解密的字符串经过 Base64 编码解码。
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="key">Key</param>
        /// <param name="iv">向量</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static string Decrypt(string decryptString, byte[] key, byte[] iv, Encoding encoding)
        {
            var datas = Convert.FromBase64String(decryptString);

            using (var aes = Aes.Create())
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Write))
            {
                cryptoStream.Write(datas, 0, datas.Length);
                cryptoStream.FlushFinalBlock();

                return encoding.GetString(memoryStream.ToArray());
            }
        }
    }
}
