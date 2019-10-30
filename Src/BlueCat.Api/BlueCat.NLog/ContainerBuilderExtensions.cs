using BlueCat.NLog.Layout;
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
            return factory;
        }
    }
}
