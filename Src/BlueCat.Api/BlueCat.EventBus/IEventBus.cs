

namespace BlueCat.EventBus
{
    /// <summary>
    /// Interface IEventBus
    /// </summary>
    /// <seealso cref="BlueCat.EventBus.IEventSubscriber" />
    /// <seealso cref="BlueCat.EventBus.IEventPublisher" />
    /// <remarks>
    /// <para>����    :xuliangjie</para>	
    /// <para>����ʱ��:2018-12-03</para>
    /// <para>������:xuliangjie</para>	
    /// <para>����ʱ��:2018-12-05</para>
    /// </remarks>
    public interface IEventBus : IEventSubscriber, IEventPublisher
    {

    }
}