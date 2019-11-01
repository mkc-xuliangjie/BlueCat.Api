
// Assembly         : BlueCat.EventBus
// Author           : xuliangjie
// Created          : 12-03-2018
//
// Last Modified By : xuliangjie
// Last Modified On : 12-06-2018

// <copyright file="IEventSubscriber.cs" company="BlueCat.EventBus">
//Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>

using System;
using System.Threading.Tasks;

namespace BlueCat.EventBus
{
    /// <summary>
    /// 事件订阅
    /// </summary>
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public interface IEventSubscriber
    {
        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="action">回调函数</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        IDisposable Subscribe<TEvent>(Func<TEvent, Task> action)
            where TEvent : class;

        /// <summary>
        /// 事件订阅
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Object to handle the event</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : class;

        /// <summary>
        /// Registers to an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <typeparam name="THandler">Type of the event handler</typeparam>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        IDisposable Subscribe<TEvent, THandler>()
            where TEvent : class
            where THandler : IEventHandler, new();

        /// <summary>
        /// Registers to an event.
        /// Same (given) instance of the handler is used for all event occurrences.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Object to handle the event</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        IDisposable Subscribe(Type eventType, IEventHandler handler);

        /// <summary>
        /// Registers to an event.
        /// Given factory is used to create/release handlers
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="factory">A factory to create/release handlers</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        IDisposable Subscribe<TEvent>(IEventHandlerFactory factory)
            where TEvent : class;

        /// <summary>
        /// Registers to an event.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="factory">A factory to create/release handlers</param>
        /// <returns>IDisposable.</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        IDisposable Subscribe(Type eventType, IEventHandlerFactory factory);

        /// <summary>
        /// Unregisters from an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="action">The action.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        void Unsubscribe<TEvent>(Func<TEvent, Task> action)
            where TEvent : class;

        /// <summary>
        /// Unregisters from an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Handler object that is registered before</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        void Unsubscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : class;

        /// <summary>
        /// Unregisters from an event.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Handler object that is registered before</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        void Unsubscribe(Type eventType, IEventHandler handler);

        /// <summary>
        /// Unregisters from an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="factory">Factory object that is registered before</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        void Unsubscribe<TEvent>(IEventHandlerFactory factory)
            where TEvent : class;

        /// <summary>
        /// Unregisters from an event.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="factory">Factory object that is registered before</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        void Unsubscribe(Type eventType, IEventHandlerFactory factory);

        /// <summary>
        /// Unregisters all event handlers of given event type.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        void UnsubscribeAll<TEvent>()
            where TEvent : class;

        /// <summary>
        /// Unregisters all event handlers of given event type.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        void UnsubscribeAll(Type eventType);
    }
}