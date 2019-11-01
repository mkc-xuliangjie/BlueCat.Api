

using BlueCat.Extensions;

namespace BlueCat.EventBus.Distributed
{
    /// <summary>
    /// Class DistributedEventBusOptions.
    /// </summary>
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public class DistributedEventBusOptions
    {
        /// <summary>
        /// Gets the handlers.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public ITypeList<IEventHandler> Handlers { get; }
        /// <summary>
        /// Gets or sets the eto mappings.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public EtoMappingDictionary EtoMappings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedEventBusOptions" /> class.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public DistributedEventBusOptions()
        {
            Handlers = new TypeList<IEventHandler>();
            EtoMappings = new EtoMappingDictionary();
        }
    }
}