using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.Extensions
{
    public sealed class NullDisposable : IDisposable
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public static NullDisposable Instance { get; } = new NullDisposable();

        /// <summary>
        /// Prevents a default instance of the <see cref="NullDisposable" /> class from being created.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        private NullDisposable()
        {

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public void Dispose()
        {

        }
    }
}
