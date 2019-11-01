
namespace BlueCat.RabbitMQ
{
    /// <summary>
    /// Class RabbitMqOptions.
    /// </summary>
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-12-19</para>
    /// </remarks>
    public class RabbitMqOptions
    {
        /// <summary>
        /// Gets the connection factories.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public RabbitMqConnections ConnectionFactories { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqOptions"/> class.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-12-19</para>
        /// </remarks>
        public RabbitMqOptions()
        {
            ConnectionFactories = new RabbitMqConnections();
        }
    }
}
