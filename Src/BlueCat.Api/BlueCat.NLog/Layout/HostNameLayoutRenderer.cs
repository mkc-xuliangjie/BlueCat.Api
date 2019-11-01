using BlueCat.GlobalCore;
using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.NLog.Layout
{
    [LayoutRenderer("hostaname")]
    public class HostNameLayoutRenderer : LayoutRenderer
    {
        internal string HostName { get { return $"{HttpContextGlobal.Ip}:{HttpContextGlobal.Current.Request.Host.Port}{HttpContextGlobal.Current.Request.Path}{HttpContextGlobal.Current.Request.QueryString}"; } }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(HostName);
        }
    }
}
