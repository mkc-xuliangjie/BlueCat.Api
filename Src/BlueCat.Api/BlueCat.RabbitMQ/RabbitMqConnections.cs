
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using RabbitMQ.Client;
using BlueCat.Extensions;

namespace BlueCat.RabbitMQ
{
    /// <summary>
    /// Class RabbitMqConnections.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{System.String, RabbitMQ.Client.ConnectionFactory}" />
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    [Serializable]
    public class RabbitMqConnections : Dictionary<string, ConnectionFactory>
    {
        /// <summary>
        /// The default connection name
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public const string DefaultConnectionName = "Default";

        /// <summary>
        /// Gets or sets the default.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        [NotNull]
        public ConnectionFactory Default
        {
            get => this[DefaultConnectionName];
            set => this[DefaultConnectionName] = Check.NotNull(value, nameof(value));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConnections"/> class.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public RabbitMqConnections()
        {
            Default = new ConnectionFactory();
        }

        /// <summary>
        /// Gets the or default.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns>ConnectionFactory.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public ConnectionFactory GetOrDefault(string connectionName)
        {
            if (TryGetValue(connectionName, out var connectionFactory))
            {
                return connectionFactory;
            }

            return Default;
        }
    }
}