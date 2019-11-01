
using System;
using RabbitMQ.Client;

namespace BlueCat.RabbitMQ
{
    /// <summary>
    /// Interface IChannelAccessor
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    public interface IChannelAccessor : IDisposable
    {
        /// <summary>
        /// Reference to the channel.
        /// Never dispose the <see cref="Channel" /> object.
        /// Instead, dispose the <see cref="IChannelAccessor" /> after usage.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        IModel Channel { get; }

        /// <summary>
        /// Name of the channel.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        string Name { get; }
    }
}