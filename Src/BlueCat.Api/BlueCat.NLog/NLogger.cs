using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using NLog;
using System;
using System.Runtime.CompilerServices;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace BlueCat.NLog
{
    public class NLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly Logger _log;

        public NLogger(string name)
        {
            _log = LogManager.GetLogger(name);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _log.IsFatalEnabled;
                case LogLevel.Debug:
                    return _log.IsDebugEnabled;
                case LogLevel.Trace:
                    return _log.IsTraceEnabled;
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Information:
                    return _log.IsInfoEnabled;
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            string message = null;
            if (null != formatter)
            {
                message = formatter(state, exception);
            }
            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        _log.Fatal(exception, message);
                        break;
                    case LogLevel.Debug:
                        _log.Debug(exception, message);
                        break;
                    case LogLevel.Trace:
                        _log.Trace(exception, message);
                        break;
                    case LogLevel.Error:
                        _log.Error(exception, message);
                        break;
                    case LogLevel.Information:
                        _log.Info(exception, message);
                        break;
                    case LogLevel.Warning:
                        _log.Warn(exception, message);
                        break;
                    default:
                        _log.Warn($"遇到未知日志级别{logLevel}");
                        _log.Info(exception, message);
                        break;
                }
            }
        }

    }
}
