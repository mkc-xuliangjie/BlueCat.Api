using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.NLog.Layout
{
    public class LayoutExtentions
    {
        public static void ReisterNlogLayout()
        {
            LayoutRenderer.Register<RequestIdLayoutRenderer>("request_id");

        }
    }
}
