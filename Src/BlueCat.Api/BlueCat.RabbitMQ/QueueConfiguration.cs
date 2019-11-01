
using System.Collections.Generic;
using JetBrains.Annotations;
using RabbitMQ.Client;

namespace BlueCat.RabbitMQ
{
    /// <summary>
    /// Class QueueConfiguration.
    /// </summary>
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    public class QueueConfiguration
    {
        /// <summary>
        /// Gets the name of the queue.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        [NotNull]
        public string QueueName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="QueueConfiguration"/> is durable.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public bool Durable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="QueueConfiguration"/> is exclusive.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public bool Exclusive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic delete].
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public IDictionary<string, object> Arguments { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueConfiguration"/> class.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="durable">if set to <c>true</c> [durable].</param>
        /// <param name="exclusive">if set to <c>true</c> [exclusive].</param>
        /// <param name="autoDelete">if set to <c>true</c> [automatic delete].</param>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public QueueConfiguration(
            [NotNull] string queueName, 
            bool durable = true, 
            bool exclusive = false, 
            bool autoDelete = false)
        {
            QueueName = queueName;
            Durable = durable;
            Exclusive = exclusive;
            AutoDelete = autoDelete;
            Arguments = new Dictionary<string, object>();
        }

        /// <summary>
        /// Declares the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>QueueDeclareOk.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public virtual QueueDeclareOk Declare(IModel channel)
        {
            return channel.QueueDeclare(
                queue: QueueName,
                durable: Durable,
                exclusive: Exclusive,
                autoDelete: AutoDelete,
                arguments: Arguments
            );
        }
    }
}