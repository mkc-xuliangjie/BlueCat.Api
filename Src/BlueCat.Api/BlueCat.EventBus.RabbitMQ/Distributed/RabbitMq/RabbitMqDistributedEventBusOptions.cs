
namespace BlueCat.EventBus.Distributed.RabbitMq
{
    /// <summary>
    /// Class RabbitMqDistributedEventBusOptions.
    /// </summary>
    /// <remarks>
    /// <para>作者    :jason</para>	
    /// <para>创建时间:2018-12-19</para>
    /// <para>最后更新:jason</para>	
    /// <para>更新时间:2018-11-30</para>
    /// </remarks>
    public class RabbitMqDistributedEventBusOptions
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-11-30</para>
        /// </remarks>
        public string ClientName { get; set; }

        /// <summary>
        /// 交换数据名称
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>	
        /// <para>创建时间:2018-12-19</para>
        /// <para>最后更新:jason</para>	
        /// <para>更新时间:2018-11-30</para>
        /// </remarks>
        public string ExchangeName { get; set; }
    }
}
