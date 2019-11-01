using BlueCat.NLog.Layout;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.NLog.DependencyInjection
{
    /// <summary>
    ///Nlog注册
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlueCatMongoNLogServices(this IServiceCollection services)
        {
            services.AddLogging(op =>
            {
                LayoutExtentions.ReisterNlogLayout();
            });

            return services;
        }
    }
}
