
// <summary></summary>

using System;
using System.Threading.Tasks;

namespace BlueCat.EventBus
{
    /// <summary>
    /// This event handler is an adapter to be able to use an action as <see cref="IEventHandler{TEvent}" /> implementation.
    /// </summary>
    /// <typeparam name="TEvent">Event type</typeparam>
    /// <seealso cref="BlueCat.EventBus.IEventHandler{TEvent}" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public class ActionEventHandler<TEvent> :
        IEventHandler<TEvent>
    {
        /// <summary>
        /// Function to handle the event.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public Func<TEvent, Task> Action { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ActionEventHandler{TEvent}" />.
        /// </summary>
        /// <param name="handler">Action to handle the event</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public ActionEventHandler(Func<TEvent, Task> handler)
        {
            Action = handler;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventData">Event data</param>
        /// <returns>Task.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public async Task HandleEventAsync(TEvent eventData)
        {
            await Action(eventData);
        }
    }
}