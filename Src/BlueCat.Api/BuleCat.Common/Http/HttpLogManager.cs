using BlueCat.Extensions.Helpers;
using BlueCat.GlobalCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuleCat.Common.Http
{
    /// <summary>
    /// 基于 Http 的日志记录，赋有额外的日志消息，如 TraceId, TraceIdentifier, Route 和 当前执行的 Class.
    /// </summary>
    public static class HttpLogManager
    {
        public static void LogTrace<T>(object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Trace(BuildLogMessage(location, message));
        }

        public static void LogTrace<T>(Exception exception, object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Trace(exception, BuildLogMessage(location, message));
        }

        public static void LogDebug<T>(object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Debug(BuildLogMessage(location, message));
        }

        public static void LogDebug<T>(Exception exception, object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Debug(exception, BuildLogMessage(location, message));
        }

        public static void LogInfo<T>(object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Info(BuildLogMessage(location, message));
        }

        public static void LogInfo<T>(Exception exception, object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Info(exception, BuildLogMessage(location, message));
        }

        public static void LogWarn<T>(object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Warn(BuildLogMessage(location, message));
        }

        public static void LogWarn<T>(Exception exception, object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Warn(exception, BuildLogMessage(location, message));
        }

        public static void LogError<T>(object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Error(BuildLogMessage(location, message));
        }

        public static void LogError<T>(Exception exception, object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Error(exception, BuildLogMessage(location, message));
        }

        public static void LogFatal<T>(object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Fatal(BuildLogMessage(location, message));
        }

        public static void LogFatal<T>(Exception exception, object message)
        {
            var location = typeof(T).FullName;
            NLog.LogManager.GetLogger(location).Fatal(exception, BuildLogMessage(location, message));
        }

        private static string BuildLogMessage(string location, object message)
        {
            var msg = new
            {
                TraceId = HttpContextGlobal.CurrentTraceId,
                HttpContextGlobal.Current?.TraceIdentifier,
                Route = $"{HttpContextGlobal.Current?.Request.Path}{HttpContextGlobal.Current?.Request.QueryString}",
                Class = location,
                Log = message
            };

            return Json.ToJson(msg);
        }
    }
}
