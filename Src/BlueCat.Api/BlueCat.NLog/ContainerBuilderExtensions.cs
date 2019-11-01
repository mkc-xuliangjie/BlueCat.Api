using BlueCat.NLog.Layout;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.NLog
{
    public static class ContainerBuilderExtensions
    {
        public static ILoggerFactory AddNLog(this ILoggerFactory factory, string nlogConfigFile = "NLog.config")
        {
            LayoutExtentions.ReisterNlogLayout();
            LogManager.LoadConfiguration(nlogConfigFile);
            factory.AddProvider(new NLogProvider());
            return factory;
        }

        public static ILoggingBuilder AddNLog(this ILoggingBuilder builder, string nlogConfigFile = "nlog.config")
        {
            LayoutExtentions.ReisterNlogLayout();
            builder.Services.AddSingleton<ILoggerProvider>(sp => new NLogProvider());
            return builder;
        }

        public static ILoggingBuilder AddNLog(this ILoggingBuilder builder, Func<string, Microsoft.Extensions.Logging.LogLevel, bool> filter, string nlogConfigFile = "nlog.config")
        {
            LayoutExtentions.ReisterNlogLayout();
            builder.AddFilter(filter);
            builder.Services.AddSingleton<ILoggerProvider>(sp => new NLogProvider());
            return builder;
        }
    }
}
