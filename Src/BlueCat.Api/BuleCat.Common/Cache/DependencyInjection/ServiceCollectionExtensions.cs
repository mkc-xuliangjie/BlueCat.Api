﻿using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace BuleCat.Common.Cache.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        //public static IServiceCollection AddRedisSettings(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddDistributedRedisCache(option =>
        //    {
        //        option.Configuration = configuration.GetSection("Redis:Host").Value;
        //        option.InstanceName = configuration.GetSection("Redis:InstanceName").Value;
        //    });

        //    return services;
        //}

        public static IServiceCollection AddCSRedisSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var options= configuration.GetSection("Redis").Get<RedisOptions>();

            string[] redisConnectionStrings = options.ConnectionStrings;

            if (services == null) throw new ArgumentNullException(nameof(services));
            if (redisConnectionStrings == null || redisConnectionStrings.Length == 0)
                throw new ArgumentNullException(nameof(redisConnectionStrings));

            CSRedisClient redisClient;
            if (redisConnectionStrings.Length == 1)
            {
                // 单机模式
                redisClient = new CSRedisClient(redisConnectionStrings[0]);
            }
            else
            {
                // 集群模式
                redisClient = new CSRedisClient(NodeRule: null, connectionStrings: redisConnectionStrings);
            }

            // 初始化 RedisHelper
            RedisHelper.Initialization(redisClient);

            //// 注册MVC分布式缓存
            //services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));

            return services;
        }
    }
}
