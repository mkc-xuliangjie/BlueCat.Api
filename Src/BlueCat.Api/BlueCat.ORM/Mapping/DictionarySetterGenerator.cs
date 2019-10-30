using System;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BlueCat.ORM.Mapping
{
    /// <summary>
    /// 	DictionarySetterGenerator
    /// </summary>
    public class DictionarySetterGenerator : IMemberSetterGenerator
    {
        #region IMemberSetterGenerator Members

        public void BeginSetMembers(CodeGenerator generator, ArgBuilder targetObject)
        {
        }

        public void SetMemberValue(CodeGenerator generator, ArgBuilder targetObject, string memberName, LocalBuilder memberValue)
        {
            generator.Ldarg(targetObject);
            generator.Load(memberName);
            generator.Ldloc(memberValue);
            if (memberValue.LocalType.IsValueType)
            {
                generator.Box(memberValue.LocalType.IsEnum ? typeof(int) : memberValue.LocalType);
            }
            generator.Call(typeof(IDictionary).GetMethod("Add"));
        }

        public void EndSetMembers(CodeGenerator generator, ArgBuilder targetObject)
        {

        }

        #endregion

        #region IMemberSetterGenerator Members

        public bool ContainsMember(string name, Type type, object outputObject)
        {
            return true;
        }

        #endregion
    }
}
