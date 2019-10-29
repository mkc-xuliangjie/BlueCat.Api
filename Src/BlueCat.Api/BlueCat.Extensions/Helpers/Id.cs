using System;
using System.Security.Cryptography;

namespace BlueCat.Extensions.Helpers
{
    /// <summary>
    /// Id生成器
    /// </summary>
    public static class Id
    {
        /// <summary>
        /// 创建Id
        /// </summary>
        public static string GetObjectId()
        {
            return BlueCat.Extensions.Helpers.Internal.ObjectId.GenerateNewStringId();
        }

        /// <summary>
        /// 取得6位的GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGuidBy6()
        {
            return System.Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace("/", "_").Replace("+", "-").Substring(0, 6);
        }

        /// <summary>
        /// 取得16位的GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGuidBy16()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        /// <summary>
        /// 取得22位的GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGuidBy22()
        {
            return System.Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace("/", "_").Replace("+", "-");
        }

        /// <summary>
        /// 取得32位的GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGuidBy32()
        {
            return System.Guid.NewGuid().ToString("N");
        }
    }
}
