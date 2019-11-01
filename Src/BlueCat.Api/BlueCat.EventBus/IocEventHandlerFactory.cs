using System;
using Microsoft.Extensions.DependencyInjection;

namespace BlueCat.EventBus
{
    /// <summary>
    /// This <see cref="IEventHandlerFactory" /> implementation is used to get/release
    /// handlers using Ioc.
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.IEventHandlerFactory" />
    /// <seealso cref="System.IDisposable" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-13</para>
    /// </remarks>
    public class IocEventHandlerFactory : IEventHandlerFactory, IDisposable
    {
        /// <summary>
        /// Gets the type of the handler.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-13</para>
        /// </remarks>
        public Type HandlerType { get; }

        /// <summary>
        /// Gets the service scope.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-13</para>
        /// </remarks>
        protected IServiceScope ServiceScope { get; }

        //TODO: Consider to inject IServiceScopeFactory instead
        /// <summary>
        /// Initializes a new instance of the <see cref="IocEventHandlerFactory" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="handlerType">Type of the handler.</param>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-13</para>
        /// </remarks>
        public IocEventHandlerFactory(IServiceProvider serviceProvider, Type handlerType)
        {
            HandlerType = handlerType;
            ServiceScope = serviceProvider
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
        }

        /// <summary>
        /// Resolves handler object from Ioc container.
        /// </summary>
        /// <returns>Resolved handler object</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-13</para>
        /// </remarks>
        public IEventHandlerDisposeWrapper GetHandler()
        {
            var scope = ServiceScope.ServiceProvider.CreateScope();
            return new EventHandlerDisposeWrapper(
                (IEventHandler) scope.ServiceProvider.GetRequiredService(HandlerType),
                () => scope.Dispose()
            );
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-13</para>
        /// </remarks>
        public void Dispose()
        {
            ServiceScope.Dispose();
        }
    }
}