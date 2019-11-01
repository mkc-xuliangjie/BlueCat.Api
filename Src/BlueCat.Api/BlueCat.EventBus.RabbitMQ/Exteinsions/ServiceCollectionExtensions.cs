using BlueCat.EventBus;
using BlueCat.EventBus.Distributed;
using BlueCat.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using BlueCat.EventBus.Distributed.RabbitMq;

namespace BlueCat.EventBus.RabbitMQ.Exteinsions
{
    /// <summary>
    /// Class IServiceCollectionExtensions.
    /// </summary>
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2019-01-15</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2019-01-15</para>
    /// </remarks>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the rabbit mq event bus.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="distributedEventBusOptions">The distributed event bus options.</param>
        /// <param name="rabbitMqOptions">
        /// options.ConnectionFactories.Default = new ConnectionFactory()
        ///       {
        ///         HostName = "192.168.70.153",
        ///UserName = "ts_admin",
        ///Password = "telsafe@123"
        /// };
        /// 
        /// </param>
        /// <param name="rabbitMqDistributedEventBusOptions">
        /// options.ClientName = "test";       
        /// options.ExchangeName = "svy";
        /// </param>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-01-15</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-01-15</para></remarks>
        public static void AddRabbitMQEventBus(this IServiceCollection services,
            Action<DistributedEventBusOptions> distributedEventBusOptions,
            Action<RabbitMqOptions> rabbitMqOptions,
            Action<RabbitMqDistributedEventBusOptions> rabbitMqDistributedEventBusOptions)
        {
            services.AddSingleton<IConnectionPool, BlueCat.RabbitMQ.ConnectionPool>();


            services.AddSingleton<IRabbitMqSerializer, BlueCat.RabbitMQ.Utf8JsonRabbitMqSerializer>();
            services.AddSingleton<IEventBus, RabbitMqDistributedEventBus>();


            services.Configure<DistributedEventBusOptions>(distributedEventBusOptions);

            services.Configure<RabbitMqOptions>(rabbitMqOptions);
            services.Configure<RabbitMqDistributedEventBusOptions>(rabbitMqDistributedEventBusOptions);
        }
    }
}
