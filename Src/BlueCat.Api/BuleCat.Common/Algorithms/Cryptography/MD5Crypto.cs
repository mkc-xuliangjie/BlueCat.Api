using System.Security.Cryptography;
using System.Text;

namespace BuleCat.Common.Algorithms.Cryptography
{
    /// <summary>
    /// MD5 加密
    /// </summary>
    public static class MD5Crypto
    {
        /// <summary>
        /// 加密。使用 UTF-8 字符编码格式。
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <returns></returns>
        public static string Encrypt(string encryptString)
        {
            return Encrypt(encryptString, Encoding.UTF8);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="encoding">字符编码格式</param>
        /// <returns></returns>
        public static string Encrypt(string encryptString, Encoding encoding)
        {
            using (var md5 = MD5.Create())
            {
                var datas = md5.ComputeHash(encoding.GetBytes(encryptString));

                var builder = new StringBuilder();
                foreach (var data in datas)
                {
                    builder.Append(data.ToString("x2")); // 每个字符以两位的形式的十六进制输出
                }

                return builder.ToString();
            }
        }
    }
}
