using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace BlueCat.ORM.Mapping
{
    public class DataRowGetterGenerator : IMemberGetterGenerator
    {
        #region IMemberGetterGenerator Members

        public void GetMemberValue(CodeGenerator generator, ArgBuilder targetObject, string memberName, ref LocalBuilder memberValue)
        {
            LocalBuilder tempLocal = generator.DeclareLocal(typeof(object), "dataColumn");
            Type targetType = memberValue.LocalType;
            Type originalType = ConvertorGeneratorHelper.GetOriginalType(targetType);

            generator.Ldarg(targetObject);
            generator.Load(memberName);
            generator.Call(typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(string) }));
            generator.Stloc(tempLocal);

            generator.Ldloc(tempLocal);
            generator.LoadMember(typeof(DBNull).GetField("Value", BindingFlags.Static | BindingFlags.Public));
            generator.If(Cmp.EqualTo);
            generator.LoadDefaultValue(targetType);
            generator.Stloc(memberValue);
            generator.Else();
            generator.Ldloc(tempLocal);
            //值是否是目标类型的对象
            generator.InternalILGenerator.Emit(OpCodes.Isinst, targetType);
            generator.InternalILGenerator.Emit(OpCodes.Ldnull);
            generator.InternalILGenerator.Emit(OpCodes.Cgt_Un);
            generator.Load(true);
            generator.If(Cmp.EqualTo);
            generator.Ldloc(tempLocal);
            if (targetType.IsValueType)
            {
                generator.UnboxAny(targetType);
            }
            generator.Stloc(memberValue);
            generator.Else();
            if (targetType == typeof(Guid) || targetType == typeof(Guid?))
            {
                generator.Ldloc(tempLocal);
                generator.InternalILGenerator.Emit(OpCodes.Isinst, typeof(string));
                generator.InternalILGenerator.Emit(OpCodes.Ldnull);
                generator.InternalILGenerator.Emit(OpCodes.Cgt_Un);
                generator.Load(true);
                generator.If(Cmp.EqualTo);
                generator.Ldloc(tempLocal);
                generator.New(typeof(Guid).GetConstructor(new Type[] { typeof(string) }));
                generator.Stloc(memberValue);
            }
            else
            {
                generator.Ldloc(tempLocal);
                generator.InternalILGenerator.Emit(OpCodes.Isinst, typeof(IConvertible));
                generator.InternalILGenerator.Emit(OpCodes.Ldnull);
                generator.InternalILGenerator.Emit(OpCodes.Cgt_Un);
                generator.Load(true);
                generator.If(Cmp.EqualTo);
                generator.Ldloc(tempLocal);
                generator.Load(originalType);
                generator.Call(typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) }));
                if (targetType.IsValueType)
                {
                    generator.UnboxAny(targetType);
                }
                generator.Stloc(memberValue);
            }
            generator.Else();
            generator.LoadDefaultValue(targetType);
            generator.Stloc(memberValue);
            generator.EndIf();
            generator.EndIf();
            generator.EndIf();
        }

        #endregion

        #region IMemberGetterGenerator Members

        public bool ContainsMember(string name, Type type, object inputObject)
        {
            DataRow dr = (DataRow)inputObject;
            return dr.Table.Columns.Contains(name);
        }

        #endregion
    }
}
