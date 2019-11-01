using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlueCat.Extensions.Reflection
{
    public static class ReflectionHelper
    {
        //TODO: Ehhance summary
        /// <summary>
        /// Checks whether <paramref name="givenType" /> implements/inherits <paramref name="genericType" />.
        /// </summary>
        /// <param name="givenType">Type to check</param>
        /// <param name="genericType">Generic type</param>
        /// <returns><c>true</c> if [is assignable to generic type] [the specified given type]; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenTypeInfo.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
        }

        //TODO: Summary
        /// <summary>
        /// Gets the implemented generic types.
        /// </summary>
        /// <param name="givenType">Type of the given.</param>
        /// <param name="genericType">Type of the generic.</param>
        /// <returns>List&lt;Type&gt;.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public static List<Type> GetImplementedGenericTypes(Type givenType, Type genericType)
        {
            var result = new List<Type>();
            AddImplementedGenericTypes(result, givenType, genericType);
            return result;
        }

        /// <summary>
        /// Adds the implemented generic types.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="givenType">Type of the given.</param>
        /// <param name="genericType">Type of the generic.</param>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        private static void AddImplementedGenericTypes(List<Type> result, Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                result.Add(givenType);
            }

            foreach (var interfaceType in givenTypeInfo.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    result.Add(interfaceType);
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return;
            }

            AddImplementedGenericTypes(result, givenTypeInfo.BaseType, genericType);
        }

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
        /// <returns>TAttribute.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public static TAttribute GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default, bool inherit = true)
            where TAttribute : Attribute
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
        /// <returns>TAttribute.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public static TAttribute GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default, bool inherit = true)
            where TAttribute : class
        {
            return memberInfo.GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? memberInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
                   ?? defaultValue;
        }

        /// <summary>
        /// Gets value of a property by it's full path from given object
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>System.Object.</returns>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        public static object GetValueByPath(object obj, Type objectType, string propertyPath)
        {
            var value = obj;
            var currentType = objectType;
            var objectPath = currentType.FullName;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
            }

            foreach (var propertyName in absolutePropertyPath.Split('.'))
            {
                var property = currentType.GetProperty(propertyName);
                value = property.GetValue(value, null);
                currentType = property.PropertyType;
            }

            return value;
        }

        /// <summary>
        /// Sets value of a property by it's full path on given object
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="value">The value.</param>
        /// <remarks>
        /// <para>作者    :jason</para>
        /// <para>创建时间:2019-11-01</para>
        /// <para>最后更新:jason</para>
        /// <para>更新时间:2019-11-01</para>
        /// </remarks>
        internal static void SetValueByPath(object obj, Type objectType, string propertyPath, object value)
        {
            var currentType = objectType;
            PropertyInfo property;
            var objectPath = currentType.FullName;
            var absolutePropertyPath = propertyPath;
            if (absolutePropertyPath.StartsWith(objectPath))
            {
                absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
            }

            var properties = absolutePropertyPath.Split('.');

            if (properties.Length == 1)
            {
                property = objectType.GetProperty(properties.First());
                property.SetValue(obj, value);
                return;
            }

            for (int i = 0; i < properties.Length - 1; i++)
            {
                property = currentType.GetProperty(properties[i]);
                obj = property.GetValue(obj, null);
                currentType = property.PropertyType;
            }

            property = currentType.GetProperty(properties.Last());
            property.SetValue(obj, value);
        }
    }
}
