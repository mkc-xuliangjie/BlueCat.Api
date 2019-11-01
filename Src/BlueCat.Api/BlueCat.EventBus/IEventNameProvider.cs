
using System;

namespace BlueCat.EventBus
{
    /// <summary>
    /// Interface IEventNameProvider
    /// </summary>
    /// <remarks>
    /// <para>作者    :xuliangjie</para>	
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>	
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public interface IEventNameProvider
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>System.String.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>	
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>	
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        string GetName(Type eventType);
    }
}