

using System;
using System.Linq;
using JetBrains.Annotations;
using BlueCat.Extensions;

namespace BlueCat.EventBus
{
    /// <summary>
    /// Class EventNameAttribute.
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
    public class EventNameAttribute : Attribute, IEventNameProvider
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public virtual string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventNameAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public EventNameAttribute([NotNull] string name)
        {
            Name = name;
            //Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        }

        /// <summary>
        /// Gets the name or default.
        /// </summary>
        /// <typeparam name="TEvent">The type of the t event.</typeparam>
        /// <returns>System.String.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public static string GetNameOrDefault<TEvent>()
        {
            return GetNameOrDefault(typeof(TEvent));
        }

        /// <summary>
        /// Gets the name or default.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>System.String.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public static string GetNameOrDefault([NotNull] Type eventType)
        {
            Check.NotNull(eventType, nameof(eventType));

            return eventType
                       .GetCustomAttributes(true)
                       .OfType<IEventNameProvider>()
                       .FirstOrDefault()
                       ?.GetName(eventType)
                   ?? eventType.FullName;
        }

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
        public string GetName(Type eventType)
        {
            return Name;
        }
    }
}
