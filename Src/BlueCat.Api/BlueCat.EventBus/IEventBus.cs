

namespace BlueCat.EventBus
{
    /// <summary>
    /// Interface IEventBus
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.IEventSubscriber" />
    /// <seealso cref="BlueCat.EventBus.IEventPublisher" />
    /// <remarks>
    /// <para>作者    :xuliangjie</para>	
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>	
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public interface IEventBus : IEventSubscriber, IEventPublisher
    {

    }
}