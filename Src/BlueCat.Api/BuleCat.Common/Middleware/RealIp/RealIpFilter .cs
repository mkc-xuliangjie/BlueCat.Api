using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace BuleCat.Common.Middleware.RealIp
{
    public class RealIpFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app => {
                app.UseMiddleware<RealIpMiddleware>();
                next(app);
            };
        }
    }
}
