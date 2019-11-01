
using System;

namespace BlueCat.EventBus
{
    /// <summary>
    /// Interface IEventHandlerDisposeWrapper
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <remarks>
    /// <para>����    :xuliangjie</para>	
    /// <para>����ʱ��:2018-12-03</para>
    /// <para>������:xuliangjie</para>	
    /// <para>����ʱ��:2018-12-05</para>
    /// </remarks>
    public interface IEventHandlerDisposeWrapper : IDisposable
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
        IEventHandler EventHandler { get; }
    }
}