
using System;
using System.Collections.Generic;

namespace BlueCat.Extensions
{
    /// <summary>
    /// A shortcut for <see cref="ITypeList{TBaseType}" /> to use object as base type.
    /// </summary>
    /// <remarks>
    /// <para>����    :jason</para>
    /// <para>����ʱ��:2019-11-01</para>
    /// <para>������:jason</para>
    /// <para>����ʱ��:2019-11-01</para>
    /// </remarks>
    public interface ITypeList : ITypeList<object>
    {

    }

    /// <summary>
    /// Extends <see cref="IList{Type}" /> to add restriction a specific base type.
    /// </summary>
    /// <typeparam name="TBaseType">Base Type of <see cref="Type" />s in this list</typeparam>
    /// <remarks>
    /// <para>����    :jason</para>
    /// <para>����ʱ��:2019-11-01</para>
    /// <para>������:jason</para>
    /// <para>����ʱ��:2019-11-01</para>
    /// </remarks>
    public interface ITypeList<in TBaseType> : IList<Type>
    {
        /// <summary>
        /// Adds a type to list.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <remarks>
        /// <para>����    :jason</para>
        /// <para>����ʱ��:2019-11-01</para>
        /// <para>������:jason</para>
        /// <para>����ʱ��:2019-11-01</para>
        /// </remarks>
        void Add<T>() where T : TBaseType;

        /// <summary>
        /// Adds a type to list if it's not already in the list.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <remarks>
        /// <para>����    :jason</para>
        /// <para>����ʱ��:2019-11-01</para>
        /// <para>������:jason</para>
        /// <para>����ʱ��:2019-11-01</para>
        /// </remarks>
        void TryAdd<T>() where T : TBaseType;

        /// <summary>
        /// Checks if a type exists in the list.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns><c>true</c> if [contains]; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>����    :jason</para>
        /// <para>����ʱ��:2019-11-01</para>
        /// <para>������:jason</para>
        /// <para>����ʱ��:2019-11-01</para>
        /// </remarks>
        bool Contains<T>() where T : TBaseType;

        /// <summary>
        /// Removes a type from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// <para>����    :jason</para>
        /// <para>����ʱ��:2019-11-01</para>
        /// <para>������:jason</para>
        /// <para>����ʱ��:2019-11-01</para>
        /// </remarks>
        void Remove<T>() where T : TBaseType;
    }
}