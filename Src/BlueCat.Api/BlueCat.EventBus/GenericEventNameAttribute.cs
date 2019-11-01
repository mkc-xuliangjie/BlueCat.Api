
using System;

namespace BlueCat.EventBus
{
    /// <summary>
    /// Class GenericEventNameAttribute.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <seealso cref="BlueCat.EventBus.IEventNameProvider" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>	
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>	
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class GenericEventNameAttribute : Attribute, IEventNameProvider
    {
        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>	
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>	
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the postfix.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>	
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>	
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public string Postfix { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="Exception">
        /// Given type is not generic: {eventType.AssemblyQualifiedName}
        /// or
        /// Given type has more than one generic argument: {eventType.AssemblyQualifiedName}
        /// </exception>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>	
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>	
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual string GetName(Type eventType)
        {
            if (!eventType.IsGenericType)
            {
                throw new Exception($"Given type is not generic: {eventType.AssemblyQualifiedName}");
            }

            var genericArguments = eventType.GetGenericArguments();
            if (genericArguments.Length > 1)
            {
                throw new Exception($"Given type has more than one generic argument: {eventType.AssemblyQualifiedName}");
            }

            var eventName = EventNameAttribute.GetNameOrDefault(genericArguments[0]);

            if (!string.IsNullOrEmpty(Prefix))
            {
                eventName = Prefix + eventName;
            }

            if (!string.IsNullOrEmpty(Postfix))
            {
                eventName = eventName + Postfix;
            }

            return eventName;
        }
    }
}