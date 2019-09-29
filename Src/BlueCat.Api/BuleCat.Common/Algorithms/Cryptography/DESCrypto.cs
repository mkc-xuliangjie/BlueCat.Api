using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BuleCat.Common.Algorithms.Cryptography
{
    /// <summary>
    /// DES 加密/解密
    /// </summary>
    public static class DESCrypto
    {
        /// <summary>
        /// 加密。使用 UTF-8 的编码格式。
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="key">Key</param>
        /// <param name="iv">向量</param>
        /// <returns></returns>
        public static string Encrypt(string encryptString, byte[] key, byte[] iv)
        {
            return Encrypt(encryptString, key, iv, Encoding.UTF8);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="key">Key</param>
        /// <param name="iv">向量</param>
        /// <returns>加密后的字符串, 以 Base64 位输出</returns>
        public static string Encrypt(string encryptString, byte[] key, byte[] iv, Encoding encoding)
        {
            var data = encoding.GetBytes(encryptString);

            using (var des = DES.Create())
            using (var mstream = new MemoryStream())
            using (var cstream = new CryptoStream(mstream, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                cstream.Write(data, 0, data.Length);
                cstream.FlushFinalBlock();

                return Convert.ToBase64String(mstream.ToArray());
            }
        }

        /// <summary>
        ///  解密。使用 UTF-8 的编码格式。
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="key">Key</param>
        /// <param name="iv">向量</param>
        /// <returns></returns>
        public static string Decrypt(string decryptString, byte[] key, byte[] iv)
        {
            return Decrypt(decryptString, key, iv);
        }

        /// <summary>
        /// 解密。
        /// 会先将要解密的字符串经过 Base64 编码解码。
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="key">Key</param>
        /// <param name="iv">向量</param>
        /// <returns>解密后的字符串</returns>
        public static string Decrypt(string decryptString, byte[] key, byte[] iv, Encoding encoding)
        {
            var data = Convert.FromBase64String(decryptString);

            using (var des = DES.Create())
            using (var mstream = new MemoryStream())
            using (var cstream = new CryptoStream(mstream, des.CreateDecryptor(key, iv), CryptoStreamMode.Write))
            {
                cstream.Write(data, 0, data.Length);
                cstream.FlushFinalBlock();

                return encoding.GetString(mstream.ToArray());
            }
        }
    }
}
