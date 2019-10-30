using System;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BlueCat.ORM.Mapping
{
    public class NameValueGetterGenerator : IMemberGetterGenerator
    {
        #region IMemberGetterGenerator Members

        public void GetMemberValue(CodeGenerator generator, ArgBuilder targetObject, string memberName, ref LocalBuilder memberValue)
        {
            LocalBuilder locValue = generator.DeclareLocal(typeof(string));
            Type targetType = memberValue.LocalType;
            Type originalType = ConvertorGeneratorHelper.GetOriginalType(targetType);

            generator.Ldarg(targetObject);
            generator.Load(memberName);
            generator.Call(typeof(NameValueCollection).GetMethod("get_Item", new Type[] { typeof(string) }));
            generator.Stloc(locValue);


            if (targetType.IsAssignableFrom(typeof(string)))
            {
                generator.Ldloc(locValue);
                generator.Stloc(memberValue);
                return;
            }
            else
            {
                generator.Ldloc(locValue);
                generator.Call(typeof(string).GetMethod("IsNullOrEmpty"));
                generator.Load(true);
                generator.If(Cmp.NotEqualTo);
                if (targetType == typeof(Guid))
                {
                    generator.Ldloc(locValue);
                    generator.New(typeof(Guid).GetConstructor(new Type[] { typeof(string) }));
                    generator.Stloc(memberValue);
                }
                else
                {
                    MethodInfo parseMethod = targetType.GetMethod("Parse", new Type[] { typeof(string) });
                    if (parseMethod != null)
                    {
                        generator.Ldloc(locValue);
                        generator.Call(parseMethod);
                        generator.Stloc(memberValue);
                    }
                    else
                    {
                        generator.Ldloc(locValue);
                        generator.Load(originalType);
                        generator.Call(typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) }));
                        if (targetType.IsValueType)
                        {
                            generator.UnboxAny(targetType);
                        }
                        generator.Stloc(memberValue);
                    }
                }
                generator.Else();
                generator.LoadDefaultValue(targetType);
                generator.Stloc(memberValue);
                generator.EndIf();
            }
        }
        #endregion

        #region IMemberGetterGenerator Members

        public bool ContainsMember(string name, Type type, object inputObject)
        {
            return true;
        }

        #endregion
    }
}
