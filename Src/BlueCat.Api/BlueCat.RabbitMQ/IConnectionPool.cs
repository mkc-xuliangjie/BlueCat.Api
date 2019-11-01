
using System;
using RabbitMQ.Client;

namespace BlueCat.RabbitMQ
{
    /// <summary>
    /// Interface IConnectionPool
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    public interface IConnectionPool : IDisposable
    {
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
        IConnection Get(string connectionName = null);
    }
}