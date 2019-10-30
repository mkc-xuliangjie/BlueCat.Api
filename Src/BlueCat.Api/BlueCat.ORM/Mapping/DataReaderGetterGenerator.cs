using System;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace BlueCat.ORM.Mapping
{
    /// <summary>
    /// 	DataReaderGetterGenerator
    /// </summary>
    public class DataReaderGetterGenerator : IMemberGetterGenerator
    {
        #region IMemberGetterGenerator Members

        public void GetMemberValue(CodeGenerator generator, ArgBuilder targetObject, string memberName, ref LocalBuilder memberValue)
        {
            MethodInfo getMethod;
            bool mustBeUnBox = false;

            switch (Type.GetTypeCode(memberValue.LocalType))
            {
                //从实测的效果来看，使用特定的函数将不会提高性能，反而性能会更低。
                case TypeCode.Boolean:
                    getMethod = typeof(IDataRecord).GetMethod("GetBoolean");
                    break;
                case TypeCode.Byte:
                    getMethod = typeof(IDataRecord).GetMethod("GetByte");
                    break;
                case TypeCode.Char:
                    getMethod = typeof(IDataRecord).GetMethod("GetChar");
                    break;
                case TypeCode.DateTime:
                    getMethod = typeof(IDataRecord).GetMethod("GetDateTime");
                    break;
                case TypeCode.Decimal:
                    getMethod = typeof(IDataRecord).GetMethod("GetDecimal");
                    break;
                case TypeCode.Double:
                    getMethod = typeof(IDataRecord).GetMethod("GetDouble");
                    break;
                case TypeCode.Single:
                    getMethod = typeof(IDataRecord).GetMethod("GetFloat");
                    break;
                case TypeCode.UInt16:
                    getMethod = typeof(IDataRecord).GetMethod("GetInt16");
                    break;
                case TypeCode.Int32:
                    getMethod = typeof(IDataRecord).GetMethod("GetInt32");
                    break;
                case TypeCode.Int64:
                    getMethod = typeof(IDataRecord).GetMethod("GetInt64");
                    break;
                case TypeCode.String:
                    getMethod = typeof(IDataRecord).GetMethod("GetString");
                    break;
                default:
                    getMethod = typeof(IDataRecord).GetMethod("GetValue");
                    mustBeUnBox = true;
                    break;
            }
            if (memberValue.LocalType == typeof(Guid))
            {
                getMethod = typeof(IDataRecord).GetMethod("GetGuid");
                mustBeUnBox = false;
            }

            var index = generator.DeclareLocal(typeof(int));

            generator.Ldarg(targetObject);
            generator.Load(memberName);
            generator.Call(typeof(IDataRecord).GetMethod("GetOrdinal"));
            generator.Stloc(index);

            generator.Ldarg(targetObject);
            generator.Ldloc(index);
            generator.Call(typeof(IDataRecord).GetMethod("IsDBNull"));
            generator.If();   //if

            generator.LoadDefaultValue(memberValue.LocalType);
            generator.Stloc(memberValue);

            generator.Else(); //else
            
            generator.Ldarg(targetObject);
            generator.Ldloc(index);
            generator.Call(getMethod);

            if (memberValue.LocalType.IsValueType && mustBeUnBox)
            {
                generator.UnboxAny(memberValue.LocalType);
            }

            generator.Stloc(memberValue);

            generator.EndIf(); //end
        }

        #endregion

        #region IMemberGetterGenerator Members

        public bool ContainsMember(string name, Type type, object inputObject)
        {
            //IDataReader.GetSchemaTable 得到的是以字段属性为列，字段为行的DataTable
            IDataReader dr = (IDataReader)inputObject;
            foreach (DataRow dataRow in dr.GetSchemaTable().Rows)
            {
                if (dataRow.ItemArray[0].ToString() == name)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
