
using System;

namespace BlueCat.EventBus
{
    /// <summary>
    /// This <see cref="IEventHandlerFactory" /> implementation is used to handle events
    /// by a transient instance object.
    /// </summary>
    /// <typeparam name="THandler">The type of the t handler.</typeparam>
    /// <seealso cref="BlueCat.EventBus.IEventHandlerFactory" />
    /// <remarks>
    /// This class always creates a new transient instance of handler.
    /// </remarks>
    internal class TransientEventHandlerFactory<THandler> : IEventHandlerFactory 
        where THandler : IEventHandler, new()
    {
        /// <summary>
        /// Creates a new instance of the handler object.
        /// </summary>
        /// <returns>The handler object</returns>
        /// <remarks>
        /// <para>����    :xuliangjie</para>	
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>	
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public IEventHandlerDisposeWrapper GetHandler()
        {
            var handler = new THandler();
            return new EventHandlerDisposeWrapper(
                handler,
                () => (handler as IDisposable)?.Dispose()
            );
        }
    }
}