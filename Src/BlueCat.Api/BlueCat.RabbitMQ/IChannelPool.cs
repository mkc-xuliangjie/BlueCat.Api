
using System;

namespace BlueCat.RabbitMQ
{
    /// <summary>
    /// Interface IChannelPool
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    public interface IChannelPool : IDisposable
    {
        /// <summary>
        /// Acquires the specified channel name.
        /// </summary>
        /// <param name="channelName">Name of the channel.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns>IChannelAccessor.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        IChannelAccessor Acquire(string channelName = null, string connectionName = null);
    }
}