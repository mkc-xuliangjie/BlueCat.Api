using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace BlueCat.ORM.Mapping
{
    public class DataRowSetterGenerator : IMemberSetterGenerator
    {
        LocalBuilder locMemberValues = null;
        LocalBuilder locDataTable = null;
        #region IMemberSetterGenerator Members
        public void BeginSetMembers(CodeGenerator generator, ArgBuilder targetObject)
        {
            locMemberValues = generator.DeclareLocal(typeof(Dictionary<string, object>), "memberValues");
            locDataTable = generator.DeclareLocal(typeof(DataTable), "dataTable");
            generator.New(typeof(Dictionary<string, object>).GetConstructor(Type.EmptyTypes));
            generator.Stloc(locMemberValues);
            generator.New(typeof(DataTable).GetConstructor(Type.EmptyTypes));
            generator.Stloc(locDataTable);
        }

        public void SetMemberValue(CodeGenerator generator, ArgBuilder targetObject, string memberName, LocalBuilder memberValue)
        {
            generator.Ldarg(targetObject);
            if (memberValue.LocalType.FullName.StartsWith("System.Nullable`1["))
            {
                generator.Ldloc(memberValue);
                generator.Load(null);
                generator.If(Cmp.EqualTo);
                generator.LoadMember(typeof(DBNull).GetField("Value"));
                generator.Stloc(memberValue);
                generator.EndIf();
            }
            generator.Load(null);
            //如果输出的DataRow对象为空，而创建DataTable结构
            generator.If(Cmp.EqualTo);
            PropertyInfo columns = typeof(DataTable).GetProperty("Columns");
            generator.Ldloc(locDataTable);
            generator.LoadMember(columns);
            generator.Load(memberName);
            generator.Ldtoken(CommonUtils.GetOriginalTypeOfNullableType(memberValue.LocalType.IsEnum ? typeof(int) : memberValue.LocalType));
            generator.Call(typeof(Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) }));
            generator.Call(typeof(DataColumnCollection).GetMethod("Add", new Type[] { typeof(string), typeof(Type) }));
            generator.Pop();

            generator.Ldloc(locMemberValues);
            generator.Load(memberName);
            generator.Ldloc(memberValue);
            if (memberValue.LocalType.IsValueType)
            {
                generator.Box(memberValue.LocalType.IsEnum ? typeof(int) : memberValue.LocalType);
            }
            generator.Call(typeof(Dictionary<string, object>).GetMethod("Add"));
            generator.Else();
            generator.Ldarg(targetObject);
            generator.Load(memberName);
            generator.Ldloc(memberValue);
            if (memberValue.LocalType.IsValueType)
            {
                generator.Box(memberValue.LocalType);
            }
            generator.Call(typeof(DataRow).GetMethod("set_Item", new Type[] { typeof(string), typeof(object) }));
            generator.EndIf();
        }

        public void EndSetMembers(CodeGenerator generator, ArgBuilder targetObject)
        {
            generator.Ldarg(targetObject);
            generator.Load(null);
            generator.If(Cmp.EqualTo);
            generator.Pop();
            LocalBuilder dataRow = generator.DeclareLocal(typeof(DataRow));
            generator.Ldloc(locDataTable);
            generator.Call(typeof(DataTable).GetMethod("NewRow"));
            generator.Stloc(dataRow);

            LocalBuilder locCurrent = generator.DeclareLocal(typeof(string));
            LocalBuilder locEnumerator = generator.DeclareLocal(typeof(Dictionary<string, object>.KeyCollection.Enumerator));

            generator.Ldloc(locMemberValues);
            generator.Call(typeof(Dictionary<string, object>).GetMethod("get_Keys"));
            generator.Call(typeof(Dictionary<string, object>.KeyCollection).GetMethod("GetEnumerator"));
            generator.Stloc(locEnumerator);
            MethodInfo getCurrentMethod = typeof(Dictionary<string, object>.KeyCollection.Enumerator).GetMethod("get_Current");
            MethodInfo moveNextMethod = typeof(Dictionary<string, object>.KeyCollection.Enumerator).GetMethod("MoveNext");
            generator.ForEach(locCurrent, typeof(string), typeof(Dictionary<string, object>.KeyCollection.Enumerator), locEnumerator, getCurrentMethod);

            generator.Ldloc(dataRow);
            generator.Ldloc(locCurrent);

            generator.Ldloc(locMemberValues);
            generator.Ldloc(locCurrent);
            generator.Call(typeof(Dictionary<string, object>).GetMethod("get_Item"));

            generator.Call(typeof(DataRow).GetMethod("set_Item", new Type[] { typeof(string), typeof(object) }));
            generator.EndForEach(moveNextMethod);
            generator.Ldloc(dataRow);
            generator.EndIf();
        }

        #endregion

        #region IMemberSetterGenerator Members

        public bool ContainsMember(string name, Type type, object outputObject)
        {
            //output datarow is null
            if (outputObject == null)
            {
                return true;
            }
            DataRow dr = (DataRow)outputObject;
            return dr.Table.Columns.Contains(name);
        }

        #endregion
    }
}
