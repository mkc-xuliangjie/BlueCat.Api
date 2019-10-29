using System;

namespace BlueCat.Extensions.Helpers {
    /// <summary>
    /// 时间操作
    /// </summary>
    public static class Time
    {
        #region 获取Unix时间戳

        /// <summary>
        /// 获取Unix时间戳
        /// </summary>
        public static long GetUnixTimestamp()
        {
            return GetUnixTimestamp(DateTime.Now);
        } 

        #endregion

        #region 获取Unix时间戳

        /// <summary>
        /// 获取Unix时间戳
        /// </summary>
        /// <param name="time">时间</param>
        public static long GetUnixTimestamp(DateTime time)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long ticks = (time - start.Add(new TimeSpan(8, 0, 0))).Ticks;
            return BlueCat.Extensions.Helpers.Convert.ToLong(ticks / TimeSpan.TicksPerSecond);
        }

        #endregion

        #region 从Unix时间戳获取时间

        /// <summary>
        /// 从Unix时间戳获取时间
        /// </summary>
        /// <param name="timestamp">Unix时间戳</param>
        public static DateTime GetTimeFromUnixTimestamp(long timestamp)
        {
            var start = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            TimeSpan span = new TimeSpan(long.Parse(timestamp + "0000000"));
            return start.Add(span).Add(new TimeSpan(8, 0, 0));
        } 

        #endregion

        #region 得到本月第一天

        /// <summary>
        /// 得到本月第一天
        /// </summary>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth()
        {
            DateTime dt = DateTime.Now;
            return FirstDayOfMonth(dt);
        }

        #endregion

        #region 得到指定日期的当月第一天

        /// <summary>
        /// 得到指定日期的当月第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(DateTime dt)
        {
            DateTime d = new DateTime(dt.Year, dt.Month, 1);
            return d;
        }

        #endregion

        #region 得到本月最后一天

        /// <summary>
        /// 得到本月最后一天
        /// </summary>
        /// <returns></returns>
        public static DateTime LastDayOfMonth()
        {
            DateTime dt = DateTime.Now;
            return LastDayOfMonth(dt);
        }

        #endregion

        #region 得到指定日期当月的最后一天

        /// <summary>
        /// 得到指定日期当月的最后一天
        /// </summary>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(DateTime dt)
        {
            DateTime d = dt.AddMonths(1).AddDays(-dt.Day);
            return d;
        }

        #endregion

        #region 获取当前时间的毫秒数

        /// <summary>
        /// 获取当前时间的毫秒数
        /// </summary>
        /// <returns></returns>
        public static long GetMillisecond()
        {
            return System.Convert.ToInt64(Math.Floor(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds));
        }

        #endregion

        #region 获取当前时间的天数

        /// <summary>
        /// 获取当前时间的天数
        /// </summary>
        /// <returns></returns>
        public static long GetDaysNumber()
        {
            return System.Convert.ToInt64(Math.Floor(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays));
        }

        #endregion

        #region 获取指定日期的天数

        /// <summary>
        /// 获取指定日期的天数
        /// </summary>
        /// <param name="appointDay">指定日期</param>
        /// <returns></returns>
        public static long GetAppointDaysNumber(DateTime appointDay)
        {
            return System.Convert.ToInt64(Math.Floor(appointDay.Subtract(new DateTime(1970, 1, 1)).TotalDays));
        }

        #endregion

        #region  DateTime转Unix

        /// <summary>
        /// DateTime转Unix
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ConvertDateTimeToUnix(DateTime time)
        {
            return ((time.ToUniversalTime().Ticks - 621355968000000000) / 10000);
        }

        #endregion

        #region  DateTime转Unix

        /// <summary>
        /// DateTime转Unix
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ConvertDateTimeToUnixTo10(DateTime time)
        {
            return ((time.ToUniversalTime().Ticks - 621355968000000000) / 10000000);
        }

        #endregion

        #region Unix转DateTime

        /// <summary>
        /// Unix转DateTime
        /// </summary>
        /// <param name="unix"></param>
        /// <returns></returns>
        public static DateTime ConvertUnixToDateTime(long unix)
        {
            DateTime startUnixTime = System.TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
            long lTime = long.Parse(unix + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return startUnixTime.Add(toNow);
        }

        #endregion

        #region Unix转DateTime(10位) 

        /// <summary>
        /// Unix转DateTime(10位) 
        /// </summary>
        /// <param name="unix"></param>
        /// <returns></returns>
        public static DateTime ConvertUnixToDateTimeTo10(long unix)
        {
            DateTime startUnixTime = System.TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
            return startUnixTime.AddSeconds(unix);
        }

        #endregion
    }
}