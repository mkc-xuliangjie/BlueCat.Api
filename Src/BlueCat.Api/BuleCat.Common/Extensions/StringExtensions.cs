using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuleCat.Common.Extensions
{
    /// <summary>
    /// <see cref="string"/> 对象的扩展类
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 将字符串转换为 int 类型，若不能转换，会抛出错误。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }

        /// <summary>
        /// 将字符串转换为 int 类型，若不能转换，会返回 null。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int? AsInt(this string str)
        {
            if (int.TryParse(str, out int value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 将字符串转换为 int 类型，若不能转换，会返回默认值。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int AsInt(this string str, int defaultValue)
        {
            return AsInt(str) ?? defaultValue;
        }

        /// <summary>
        ///  将字符串转换为 DateTime 类型，若不能转换，会抛出错误。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str)
        {
            return DateTime.Parse(str);
        }

        /// <summary>
        /// 将字符串转换为 DateTime 类型，若不能转换，会返回 null。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime? AsDateTime(this string str)
        {
            if (DateTime.TryParse(str, out DateTime value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 将字符串转换为 DateTime 类型，若不能转换，会返回默认值。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime AsDateTime(this string str, DateTime defaultValue)
        {
            return AsDateTime(str) ?? defaultValue;
        }

        /// <summary>
        /// 将字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">要反序列化的对象类型</typeparam>
        /// <param name="str">反序列化的字符串</param>
        /// <param name="settings">反序列化设置选项</param>
        /// <returns></returns>
        public static T ToObject<T>(this string str, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(str, settings ?? new JsonSerializerSettings());
        }

        /// <summary>
        /// 将字符串反序列化为对象
        /// </summary>
        /// <param name="str">反序列化的字符串</param>
        /// <param name="objectType">要反序列化的类型</param>
        /// <param name="settings">反序列化设置选项</param>
        /// <returns></returns>
        public static object ToObject(this string str, Type objectType, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject(str, objectType, settings ?? new JsonSerializerSettings());
        }

        /// <summary>
        /// 将字符串反序列化为指定类型的对象
        /// </summary>
        /// <param name="str">String.</param>
        /// <param name="settings">Json Settings.</param>
        /// <typeparam name="T">要反序列化的的对象类型</typeparam>
        public static T AsObject<T>(this string str, JsonSerializerSettings settings = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return default(T);

            try
            {
                return JsonConvert.DeserializeObject<T>(str, settings ?? new JsonSerializerSettings());
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 将字符串反序列化为指定类型的对象
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="objectType">要反序列化的的对象类型</param>
        /// <param name="settings">Json Settings.</param>
        /// <returns></returns>
        public static object AsObject(this string str, Type objectType, JsonSerializerSettings settings = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            try
            {
                return JsonConvert.DeserializeObject(str, objectType, settings ?? new JsonSerializerSettings());
            }
            catch
            {
                return null;
            }
        }
    }
}
