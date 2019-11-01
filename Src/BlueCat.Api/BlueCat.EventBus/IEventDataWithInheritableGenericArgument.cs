

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
    /// <para>作者    :xuliangjie</para>	
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>	
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public interface IEventDataWithInheritableGenericArgument
    {
        /// <summary>
        /// Gets arguments to create this class since a new instance of this class is created.
        /// </summary>
        /// <returns>Constructor arguments</returns>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>	
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>	
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        object[] GetConstructorArgs();
    }
}