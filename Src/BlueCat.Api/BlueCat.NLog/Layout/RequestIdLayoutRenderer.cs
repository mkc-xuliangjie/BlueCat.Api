using BlueCat.GlobalCore;
using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.NLog.Layout
{
    [LayoutRenderer("request_id")]
    public class RequestIdLayoutRenderer : LayoutRenderer
    {
        internal string RequestID { get { return HttpContextGlobal.CurrentTraceId; } }
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(RequestID);
        }
    }
}
