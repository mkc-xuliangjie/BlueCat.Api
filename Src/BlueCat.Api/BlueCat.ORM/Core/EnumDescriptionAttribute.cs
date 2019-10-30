using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System
{
    /// <summary>
    /// EnumDescriptionAttribute
    /// An Chinese version introduction to the usage of this class: 
    /// http://www.cnblogs.com/teddyma/archive/2007/05/26/759842.html
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false, Inherited=true)]
    public class EnumDescriptionAttribute : Attribute
    {
        private string defaultDesc;

        /// <summary>
        /// The default desc
        /// </summary>
        public string DefaultDescription
        {
            get
            {
                return defaultDesc;
            }
            set
            {
                defaultDesc = value;
            }
        }

        /// <summary>
        /// Get desc of specific enum value
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public virtual string GetDescription(object enumValue)
        {
            Check.Require(enumValue != null, "enumValue could not be null.");

            return DefaultDescription ?? enumValue.ToString();
        }

        /// <summary>
        /// Get desc of specific enum value of specific enum type
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enumIntValue"></param>
        /// <returns></returns>
        public static string GetDescription(Type enumType, int enumIntValue)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Dictionary<int, string> descs = EnumDescriptionAttribute.GetDescriptions(enumType);
            Dictionary<int, string>.Enumerator en = descs.GetEnumerator();
            while (en.MoveNext())
            {
                if ((enumIntValue & en.Current.Key) == en.Current.Key)
                {
                    if (sb.Length == 0)
                    {
                        sb.Append(en.Current.Value);
                    }
                    else
                    {
                        sb.Append(',');
                        sb.Append(en.Current.Value);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get descs of specific enum type
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetDescriptions(Type enumType)
        {
            Check.Require(enumType != null && enumType.IsEnum, "enumType must be an enum type.");

            FieldInfo[] fields = enumType.GetFields();
            Dictionary<int, string> descs = new Dictionary<int, string>();
            for (int i = 1; i < fields.Length; ++i)
            {
                object fieldValue = Enum.Parse(enumType, fields[i].Name);
                object[] attrs = fields[i].GetCustomAttributes(true);
                bool findAttr = false;
                foreach (object attr in attrs)
                {
                    if (typeof(EnumDescriptionAttribute).IsAssignableFrom(attr.GetType()))
                    {
                        descs.Add((int)fieldValue, ((EnumDescriptionAttribute)attr).GetDescription(fieldValue));
                        findAttr = true;
                        break;
                    }
                }
                if (!findAttr)
                {
                    descs.Add((int)fieldValue, fieldValue.ToString());
                }
            }

            return descs;
        }
    }
}
