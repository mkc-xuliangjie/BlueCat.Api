using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchangeRedis.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStackExchangeRedis(this IServiceCollection services, IConfiguration configuration)
        {
            StackExchangeRedisHelper.redisConnectionStr= configuration.GetSection("Redis:Host").Value;
            // add context
            services.AddSingleton<StackExchangeRedisHelper>();

            services.AddSingleton<IRedisCache, Redis>();

            return services;
        }
    }
}
