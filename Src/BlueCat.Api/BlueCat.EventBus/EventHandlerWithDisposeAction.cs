

using System;

namespace BlueCat.EventBus
{
    /// <summary>
    /// Class EventHandlerDisposeWrapper.
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.IEventHandlerDisposeWrapper" />
    /// <remarks>
    /// <para>����    :xuliangjie</para>
    /// <para>����ʱ��:2018-12-03</para>
    /// <para>������:xuliangjie</para>
    /// <para>����ʱ��:2018-12-05</para>
    /// </remarks>
    public class EventHandlerDisposeWrapper : IEventHandlerDisposeWrapper
    {
        /// <summary>
        /// Gets the event handler.
        /// </summary>
        /// <remarks>
        /// <para>����    :xuliangjie</para>
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public IEventHandler EventHandler { get; }

        /// <summary>
        /// The dispose action
        /// </summary>
        /// <remarks>
        /// <para>����    :xuliangjie</para>
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        private readonly Action _disposeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerDisposeWrapper" /> class.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="disposeAction">The dispose action.</param>
        /// <remarks>
        /// <para>����    :xuliangjie</para>
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public EventHandlerDisposeWrapper(IEventHandler eventHandler, Action disposeAction = null)
        {
            _disposeAction = disposeAction;
            EventHandler = eventHandler;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// <para>����    :xuliangjie</para>
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        public void Dispose()
        {
            _disposeAction?.Invoke();
        }
    }
}