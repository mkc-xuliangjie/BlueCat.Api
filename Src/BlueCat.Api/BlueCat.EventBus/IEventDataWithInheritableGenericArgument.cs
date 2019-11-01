

namespace BlueCat.EventBus
{
    /// <summary>
    /// This interface must be implemented by event data classes that
    /// has a single generic argument and this argument will be used by inheritance.
    /// For example;
    /// Assume that Student inherits From Person. When trigger an EntityCreatedEventData{Student},
    /// EntityCreatedEventData{Person} is also triggered if EntityCreatedEventData implements
    /// this interface.
    /// </summary>
    /// <remarks>
    /// <para>����    :xuliangjie</para>	
    /// <para>����ʱ��:2018-12-03</para>
    /// <para>������:xuliangjie</para>	
    /// <para>����ʱ��:2018-12-05</para>
    /// </remarks>
    public interface IEventDataWithInheritableGenericArgument
    {
        /// <summary>
        /// Gets arguments to create this class since a new instance of this class is created.
        /// </summary>
        /// <returns>Constructor arguments</returns>
        /// <remarks>
        /// <para>����    :xuliangjie</para>	
        /// <para>����ʱ��:2018-12-03</para>
        /// <para>������:xuliangjie</para>	
        /// <para>����ʱ��:2018-12-05</para>
        /// </remarks>
        object[] GetConstructorArgs();
    }
}