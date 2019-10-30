using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace BlueCat.ORM.Mapping
{
    /// <summary>
    /// convert handler
    /// </summary>
    /// <param name="inputObject"></param>
    /// <param name="outputObject"></param>
    /// <returns></returns>
    public delegate object ConvertHandler(object inputObject, object outputObject);

    sealed class ConvertorGeneratorHelper
    {
        public static string GetCurrentKey(Dictionary<int, string> mappingOrders, int currentCount, List<string> keyList)
        {
            string key = null;
            //找出不在排序列中
            foreach (string k in keyList)
            {
                if (!mappingOrders.ContainsValue(k))
                {
                    key = k;
                    break;
                }
            }
            if (mappingOrders.ContainsKey(currentCount))
            {
                key = mappingOrders[currentCount];
            }
            keyList.Remove(key);
            return key;
        }

        public static Type GetMemberType(MemberInfo member)
        {
            Type type = null;
            if (member.MemberType == MemberTypes.Field)
            {
                type = ((FieldInfo)member).FieldType;
            }
            else
            {
                type = ((PropertyInfo)member).PropertyType;
            }
            return type;
        }

        public static Type GetOriginalType(Type type)
        {
            Type originalType = type.IsEnum ? typeof(int) : type;
            originalType = CommonUtils.GetOriginalTypeOfNullableType(originalType);
            return originalType;
        }

        public static Dictionary<string, MemberInfo> GetMembers(Type type)
        {
            Dictionary<string, MemberInfo> members = new Dictionary<string, MemberInfo>();
            Type objType = type;
            while (objType != null && objType != typeof(Object) && objType != typeof(ValueType))
            {
                foreach (PropertyInfo property in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        members.Add(property.Name, property);
                    }
                }
                foreach (FieldInfo field in objType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!field.IsInitOnly)
                    {
                        members.Add(field.Name, field);
                    }
                }
                objType = objType.BaseType;
            }
            return members;
        }

        public static MemberInfo GetMemberInfo(Type type, string name)
        {
            MemberInfo member = type.GetProperty(name);
            if (member == null)
            {
                member = type.GetField(name);
            }
            return member;
        }

        public static Dictionary<TValue, TKey> RevertDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            Dictionary<TValue, TKey> dic = new Dictionary<TValue, TKey>();
            foreach (TKey key in dictionary.Keys)
            {
                dic.Add(dictionary[key], key);
            }
            return dic;
        }
    }

    /// <summary>
    /// 代码生成驱动器
    /// </summary>
    public abstract class GeneratorDriver
    {
        public GeneratorDriver() { }
        private IMemberGetterGenerator memberGetterGenerator;
        public IMemberGetterGenerator MemberGetterGenerator
        {
            get { return memberGetterGenerator; }
        }
        private IMemberSetterGenerator memberSetterGenerator;
        public IMemberSetterGenerator MemberSetterGenerator
        {
            get { return memberSetterGenerator; }
        }

        public GeneratorDriver(IMemberGetterGenerator memberGetterGenerator, IMemberSetterGenerator memberSetterGenerator)
        {
            this.memberGetterGenerator = memberGetterGenerator;
            this.memberSetterGenerator = memberSetterGenerator;
        }

        public abstract ConvertHandler GetConvertHandler(Type inputType, Type outputType, object inputObject, object outputObject, Dictionary<string, string> mappingNames, List<string> ignoreList, Dictionary<int, string> mappingOrders, bool mappingSpecifiedOnly);
    }
}
