
using System;

namespace BlueCat.EventBus
{
    /// <summary>
    /// Interface IEventHandlerDisposeWrapper
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>	
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>	
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public interface IEventHandlerDisposeWrapper : IDisposable
    {
        /// <summary>
        /// Gets the event handler.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>	
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>	
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        IEventHandler EventHandler { get; }
    }
}