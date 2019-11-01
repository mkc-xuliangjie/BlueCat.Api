
namespace BlueCat.EventBus
{
    /// <summary>
    /// This <see cref="IEventHandlerFactory" /> implementation is used to handle events
    /// by a single instance object.
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.IEventHandlerFactory" />
    /// <remarks>
    /// This class always gets the same single instance of handler.
    /// </remarks>
    public class SingleInstanceHandlerFactory : IEventHandlerFactory
    {
        /// <summary>
        /// The event handler instance.
        /// </summary>
        /// <remarks>
        /// <para>����    :xuliangjie</para>	
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>	
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public IEventHandler HandlerInstance { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleInstanceHandlerFactory"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <remarks>
        /// <para>����    :xuliangjie</para>	
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>	
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public SingleInstanceHandlerFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        /// <summary>
        /// Gets the handler.
        /// </summary>
        /// <returns>IEventHandlerDisposeWrapper.</returns>
        /// <remarks>
        /// <para>����    :xuliangjie</para>	
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>	
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public IEventHandlerDisposeWrapper GetHandler()
        {
            return new EventHandlerDisposeWrapper(HandlerInstance);
        }
    }
}