

using System;
using System.Collections.Generic;

namespace BlueCat.EventBus.Distributed
{
    /// <summary>
    /// Class EtoMappingDictionary.
    /// </summary>
    /// <remarks>
    /// <para>作者    :xuliangjie</para>
    /// <para>创建时间:2018-12-03</para>
    /// <para>最后更新:xuliangjie</para>
    /// <para>更新时间:2018-12-05</para>
    /// </remarks>
    public class EtoMappingDictionary : Dictionary<Type, Type>
    {
        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the t entity.</typeparam>
        /// <typeparam name="TEntityEto">The type of the t entity eto.</typeparam>
        /// <remarks>
        /// <para>作者    :xuliangjie</para>
        /// <para>创建时间:2018-12-03</para>
        /// <para>最后更新:xuliangjie</para>
        /// <para>更新时间:2018-12-05</para>
        /// </remarks>
        public void Add<TEntity, TEntityEto>()
        {
            this[typeof(TEntity)] = typeof(TEntityEto);
        }
    }
}