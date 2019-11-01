using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BlueCat.Extensions
{
    /// <summary>
    /// A shortcut for <see cref="TypeList{TBaseType}" /> to use object as base type.
    /// </summary>
    /// <seealso cref="BlueCat.Extensions.ITypeList" />
    /// <remarks>
    /// <para>作者    :jason</para>
    /// <para>创建时间:2019-11-01</para>
    /// <para>最后更新:jason</para>
    /// <para>更新时间:2019-11-01</para>
    /// </remarks>
    public class TypeList : TypeList<object>, ITypeList
    {
    }

    /// <summary>
    /// Extends <see cref="List{Type}" /> to add restriction a specific base type.
    /// </summary>
    /// <typeparam name="TBaseType">Base Type of <see cref="Type" />s in this list</typeparam>
    /// <seealso cref="BlueCat.Extensions.ITypeList{TBaseType}" />
    /// <remarks>
    /// <para>作者    :jason</para>
    /// <para>创建时间:2019-11-01</para>
    /// <para>最后更新:jason</para>
    /// <para>更新时间:2019-11-01</para>
    /// </remarks>
    public class TypeList<TBaseType> : ITypeList<TBaseType>
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public int Count => _typeList.Count;

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets or sets the <see cref="Type" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Type.</returns>
        /// <remarks><para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para></remarks>
        public Type this[int index]
        {
            get { return _typeList[index]; }
            set
            {
                CheckType(value);
                _typeList[index] = value;
            }
        }

        /// <summary>
        /// The type list
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        private readonly List<Type> _typeList;

        /// <summary>
        /// Creates a new <see cref="TypeList{T}" /> object.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public TypeList()
        {
            _typeList = new List<Type>();
        }

        /// <summary>
        /// Adds a type to list.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public void Add<T>() where T : TBaseType
        {
            _typeList.Add(typeof(T));
        }

        /// <summary>
        /// Adds a type to list if it's not already in the list.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public void TryAdd<T>() where T : TBaseType
        {
            if (Contains<T>())
            {
                return;
            }

            Add<T>();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public void Add(Type item)
        {
            CheckType(item);
            _typeList.Add(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"></see> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public void Insert(int index, Type item)
        {
            CheckType(item);
            _typeList.Insert(index, item);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <returns>The index of <paramref name="item">item</paramref> if found in the list; otherwise, -1.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public int IndexOf(Type item)
        {
            return _typeList.IndexOf(item);
        }

        /// <summary>
        /// Checks if a type exists in the list.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns><c>true</c> if [contains]; otherwise, <c>false</c>.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public bool Contains<T>() where T : TBaseType
        {
            return Contains(typeof(T));
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>true if <paramref name="item">item</paramref> is found in the otherwise, false.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public bool Contains(Type item)
        {
            return _typeList.Contains(item);
        }

        /// <summary>
        /// Removes a type from list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public void Remove<T>() where T : TBaseType
        {
            _typeList.Remove(typeof(T));
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>true if <paramref name="item">item</paramref> was successfully removed from the otherwise, false. This method also returns false if <paramref name="item">item</paramref> is not found in the original .</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public bool Remove(Type item)
        {
            return _typeList.Remove(item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public void RemoveAt(int index)
        {
            _typeList.RemoveAt(index);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public void Clear()
        {
            _typeList.Clear();
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public void CopyTo(Type[] array, int arrayIndex)
        {
            _typeList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        /// <inheritdoc />
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public IEnumerator<Type> GetEnumerator()
        {
            return _typeList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _typeList.GetEnumerator();
        }

        /// <summary>
        /// Checks the type.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentException">Given type ({item.AssemblyQualifiedName}) should be instance of {typeof(TBaseType).AssemblyQualifiedName} - item</exception>
        /// <exception cref="System.ArgumentException">Given type ({item.AssemblyQualifiedName}) should be instance of {typeof(TBaseType).AssemblyQualifiedName} - item</exception>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        private static void CheckType(Type item)
        {
            if (!typeof(TBaseType).GetTypeInfo().IsAssignableFrom(item))
            {
                throw new ArgumentException($"Given type ({item.AssemblyQualifiedName}) should be instance of {typeof(TBaseType).AssemblyQualifiedName} ", nameof(item));
            }
        }
    }
}