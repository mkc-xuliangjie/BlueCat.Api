

using BlueCat.Extensions;

namespace BlueCat.EventBus.Distributed
{
    /// <summary>
    /// Class DistributedEventBusOptions.
    /// </summary>
    /// <remarks>
    /// <para>����    :xuliangjie</para>
    /// <para>����ʱ��:2018-12-03</para>
    /// <para>������:xuliangjie</para>
    /// <para>����ʱ��:2018-12-05</para>
    /// </remarks>
    public class DistributedEventBusOptions
    {
        /// <summary>
        /// Gets the handlers.
        /// </summary>
        /// <remarks>
        /// <para>����    :xuliangjie</para>
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public ITypeList<IEventHandler> Handlers { get; }
        /// <summary>
        /// Gets or sets the eto mappings.
        /// </summary>
        /// <remarks>
        /// <para>����    :xuliangjie</para>
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public EtoMappingDictionary EtoMappings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedEventBusOptions" /> class.
        /// </summary>
        /// <remarks>
        /// <para>����    :xuliangjie</para>
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public DistributedEventBusOptions()
        {
            Handlers = new TypeList<IEventHandler>();
            EtoMappings = new EtoMappingDictionary();
        }
    }
}