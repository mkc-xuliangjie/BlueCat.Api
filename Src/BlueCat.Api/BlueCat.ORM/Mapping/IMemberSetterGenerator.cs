using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace BlueCat.ORM.Mapping
{
    /// <summary>
    /// Setter代码生成器
    /// </summary>
    public interface IMemberSetterGenerator
    {
        /// <summary>
        /// contains member
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="inputObject">The input object.</param>
        /// <returns></returns>
        bool ContainsMember(string name, Type type, object outputObject);

        void BeginSetMembers(CodeGenerator generator, ArgBuilder targetObject);
        /// <summary>
        /// 	
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="targetObject">The target object.output object 参数</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="memberValue">The member value.</param>
        void SetMemberValue(CodeGenerator generator, ArgBuilder targetObject, string memberName, LocalBuilder memberValue);

        /// <summary>
        /// Ends the set members.
        /// </summary>
        /// <param name="generator">The generator.</param>
        void EndSetMembers(CodeGenerator generator, ArgBuilder targetObject);
    }
}
