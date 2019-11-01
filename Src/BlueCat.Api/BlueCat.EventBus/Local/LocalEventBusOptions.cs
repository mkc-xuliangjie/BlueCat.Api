

using BlueCat.Extensions;


namespace BlueCat.EventBus.Local
{
    /// <summary>
    /// Class LocalEventBusOptions.
    /// </summary>
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public class LocalEventBusOptions
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
        /// Initializes a new instance of the <see cref="LocalEventBusOptions" /> class.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public LocalEventBusOptions()
        {
            Handlers = new TypeList<IEventHandler>();
        }
    }
}