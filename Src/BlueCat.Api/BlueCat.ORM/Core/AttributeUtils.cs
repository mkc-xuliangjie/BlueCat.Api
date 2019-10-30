using System;
using System.Collections.Generic;
using System.Reflection;

namespace System
{
    /// <summary>
    /// The AttributeUtils
    /// </summary>
    public sealed class AttributeUtils
    {
        private AttributeUtils() { }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="findChildAttributes">if set to <c>true</c> [find child attributes].</param>
        /// <returns></returns>
        public static T GetAttribute<T>(MemberInfo member, bool findChildAttributes)
            where T : Attribute
        {
            object[] attrs = member.GetCustomAttributes(true);

            if (attrs != null && attrs.Length > 0)
            {
                for (int i = 0; i < attrs.Length; ++i)
                {
                    if (findChildAttributes ? typeof(T).IsAssignableFrom(attrs[i].GetType()) : typeof(T) == attrs[i].GetType())
                    {
                        return (T)attrs[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static T GetAttribute<T>(MemberInfo member)
            where T : Attribute
        {
            return GetAttribute<T>(member, false);
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="findChildAttributes">if set to <c>true</c> [find child attributes].</param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(MemberInfo member, bool findChildAttributes)
            where T : Attribute
        {
            List<T> list = new List<T>();

            object[] attrs = member.GetCustomAttributes(true);

            if (attrs != null && attrs.Length > 0)
            {
                for (int i = 0; i < attrs.Length; ++i)
                {
                    if (findChildAttributes ? typeof(T).IsAssignableFrom(attrs[i].GetType()) : typeof(T) == attrs[i].GetType())
                    {
                        list.Add((T)attrs[i]);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(MemberInfo member)
            where T : Attribute
        {
            return GetAttributes<T>(member, false);
        }
    }
}
