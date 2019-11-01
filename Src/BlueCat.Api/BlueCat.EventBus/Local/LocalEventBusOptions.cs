

using BlueCat.Extensions;


namespace BlueCat.EventBus.Local
{
    /// <summary>
    /// Class LocalEventBusOptions.
    /// </summary>
    /// <remarks>
    /// <para>����    :xuliangjie</para>
    /// <para>����ʱ��:2018-12-03</para>
    /// <para>������:xuliangjie</para>
    /// <para>����ʱ��:2018-12-05</para>
    /// </remarks>
    public class LocalEventBusOptions
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
        /// Initializes a new instance of the <see cref="LocalEventBusOptions" /> class.
        /// </summary>
        /// <remarks>
        /// <para>����    :xuliangjie</para>
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public LocalEventBusOptions()
        {
            Handlers = new TypeList<IEventHandler>();
        }
    }
}