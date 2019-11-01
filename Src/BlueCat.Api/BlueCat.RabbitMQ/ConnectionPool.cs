
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace BlueCat.RabbitMQ
{
    /// <summary>
    /// Class ConnectionPool.
    /// </summary>
    /// <seealso cref="BlueCat.RabbitMQ.IConnectionPool" />
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    public class ConnectionPool : IConnectionPool
    {
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected RabbitMqOptions Options { get; }

        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        protected ConcurrentDictionary<string, IConnection> Connections { get; }

        /// <summary>
        /// The is disposed
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPool"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public ConnectionPool(IOptions<RabbitMqOptions> options)
        {
            Options = options.Value;
            Connections = new ConcurrentDictionary<string, IConnection>();
        }

        /// <summary>
        /// Gets the specified connection name.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns>IConnection.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public virtual IConnection Get(string connectionName = null)
        {
            connectionName = connectionName
                             ?? RabbitMqConnections.DefaultConnectionName;

            return Connections.GetOrAdd(
                connectionName,
                (key)=>Options
                    .ConnectionFactories
                    .GetOrDefault(key)
                    .CreateConnection()
            );
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach (var connection in Connections.Values)
            {
                try
                {
                    connection.Dispose();
                }
                catch
                {

                }
            }

            Connections.Clear();
        }
    }
}