using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BlueCat.Extensions
{
    /// <summary>
    /// Class Check.
    /// </summary>
    /// <remarks>
    /// <para>作者    :jason</para>
    /// <para>创建时间:2019-11-01</para>
    /// <para>最后更新:jason</para>
    /// <para>更新时间:2019-11-01</para>
    /// </remarks>
    [DebuggerStepThrough]
    public static class Check
    {
        /// <summary>
        /// Nots the null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>T.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(T value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Nots the null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="message">The message.</param>
        /// <returns>T.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(T value, [InvokerParameterName] [NotNull] string parameterName, string message)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName, message);
            }

            return value;
        }

        /// <summary>
        /// Nots the null or white space.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        [ContractAnnotation("value:null => halt")]
        public static string NotNullOrWhiteSpace(string value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// Nots the null or empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        [ContractAnnotation("value:null => halt")]
        public static string NotNullOrEmpty(string value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
            }

            return value;
        }

    }
}
