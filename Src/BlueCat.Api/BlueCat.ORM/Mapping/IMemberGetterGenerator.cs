using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace BlueCat.ORM.Mapping
{
    /// <summary>
    /// Getter代码生成器
    /// </summary>
    public interface IMemberGetterGenerator
    {
        /// <summary>
        /// 判断是是否包含某个属性
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="inputObject">The input object.</param>
        /// <returns></returns>
        bool ContainsMember(string name, Type type, object inputObject);
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="targetObject">The target object.input object 参数</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="memberValue">The member value.</param>
        void GetMemberValue(CodeGenerator generator, ArgBuilder targetObject, string memberName, ref LocalBuilder memberValue);
    }

}
