using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Reflection.Emit
{
    /// <summary>
    /// Delegate for calling static method/property/field
    /// </summary>
    /// <param name="paramObjs">The parameters passing to the invoking method.</param>
    /// <returns>The return value.</returns>
    public delegate object StaticDynamicMethodProxyHandler(object[] paramObjs);

    /// <summary>
    /// Delegate for calling non-static method/property/field
    /// </summary>
    /// <param name="ownerInstance">The object instance owns the invoking method.</param>
    /// <param name="paramObjs">The parameters passing to the invoking method.</param>
    /// <returns>The return value.</returns>
    public delegate object DynamicMethodProxyHandler(object ownerInstance, object[] paramObjs);

    /// <summary>
    /// DynamicMethodFactory
    /// </summary>
    public class DynamicMethodFactory
    {
        #region Private Methods

        private static MethodInfo MakeMethodGeneric(MethodInfo genericMethodInfo, Type[] genericParameterTypes)
        {
            if (genericMethodInfo == null)
            {
                throw new ArgumentNullException("genericMethodInfo");
            }

            MethodInfo makeGenericMethodInfo;
            if (genericParameterTypes != null && genericParameterTypes.Length > 0)
            {
                makeGenericMethodInfo = genericMethodInfo.MakeGenericMethod(genericParameterTypes);
            }
            else
            {
                makeGenericMethodInfo = genericMethodInfo;
            }
            return makeGenericMethodInfo;
        }

        private static void LoadParameters(CodeGenerator gen, ParameterInfo[] pis, bool isMethodStatic)
        {
            Check.Require(gen, "gen");

            if (pis != null)
            {
                for (int i = 0; i < pis.Length; ++i)
                {
                    if (isMethodStatic)
                    {
                        gen.Ldarg(0);
                    }
                    else
                    {
                        gen.Ldarg(1);
                    }
                    gen.Ldc(i);

                    Type srcType = pis[i].ParameterType;
                    string str = srcType.ToString();
                    if (str.EndsWith("&"))
                    {
                        srcType = CommonUtils.GetType(str.Substring(0, str.Length - 1));
                    }

                    if (str.EndsWith("&")) //ref or out param
                    {
                        if (srcType.IsValueType && (pis[i].Attributes & ParameterAttributes.Out) != ParameterAttributes.Out) //ref value param
                        {
                            gen.Ldelem(typeof(object));
                            gen.Unbox(srcType);
                        }
                        else
                        {
                            if (srcType.IsValueType && srcType != typeof(object)) //out value param
                            {
                                gen.LoadDefaultValue(srcType);
                                gen.Box(srcType);
                                gen.Stelem(typeof(object));

                                if (isMethodStatic)
                                {
                                    gen.Ldarg(0);
                                }
                                else
                                {
                                    gen.Ldarg(1);
                                }
                                gen.Ldc(i);
                                gen.Ldelem(typeof(object));
                                gen.Unbox(srcType);
                            }
                            else //ref or out class param
                            {
                                gen.Ldelema(typeof(object));
                            }
                        }
                    }
                    else
                    {
                        gen.Ldelem(typeof(object));

                        if (srcType.IsValueType)
                        {
                            gen.UnboxAny(srcType);
                        }
                        else if (srcType != typeof(object))
                        {
                            gen.Castclass(srcType);
                        }
                    }
                }
            }
        }

        private static void CastValueToObject(CodeGenerator gen, Type valueType)
        {
            if (valueType == typeof(void))
            {
                gen.Load(null);
            }
            else if (valueType.IsValueType)
            {
                gen.Box(valueType);
            }
            else if (valueType != typeof(object))
            {
                gen.Castclass(typeof(object));
            }
        }

        #endregion

        #region Get static delegate

        /// <summary>
        /// Do GetStaticMethodDelegate
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="genericMethodInfo"></param>
        /// <param name="genericParameterTypes"></param>
        /// <returns></returns>
        protected static StaticDynamicMethodProxyHandler DoGetStaticMethodDelegate(
            Module targetModule, 
            MethodInfo genericMethodInfo, 
            params Type[] genericParameterTypes)
        {
            #region Check preconditions

            Check.Require(targetModule, "targetModule");
            Check.Require(genericMethodInfo, "genericMethodInfo");
            Check.Require((genericParameterTypes == null && genericMethodInfo.GetGenericArguments().Length == 0) || 
                genericParameterTypes.Length == genericMethodInfo.GetGenericArguments().Length,
                "The number of generic type parameter of genericMethodInfo and the input types must equal!");
            Check.Require(genericMethodInfo.IsStatic, "genericMethodInfo must be static here!");

            #endregion

            //Create a dynamic method proxy delegate used to call the specified methodinfo
            CodeGenerator gen = new CodeGenerator(targetModule);
            gen.BeginMethod("dm" + Guid.NewGuid().ToString("N"), typeof(StaticDynamicMethodProxyHandler));
            MethodInfo makeGenericMethodInfo = MakeMethodGeneric(genericMethodInfo, genericParameterTypes);
            LoadParameters(gen, makeGenericMethodInfo.GetParameters(), true);
            gen.Call(makeGenericMethodInfo);
            CastValueToObject(gen, makeGenericMethodInfo.ReturnType);

            return (StaticDynamicMethodProxyHandler)gen.EndMethod();
        }

        /// <summary>
        /// Do Get Static Field Get Delegate
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected static StaticDynamicMethodProxyHandler DoGetStaticFieldGetDelegate(
            Module targetModule,
            FieldInfo fieldInfo
            )
        {
            Check.Require(targetModule, "targetModule");
            Check.Require(fieldInfo, "fieldInfo");
            Check.Require(fieldInfo.IsStatic, "fieldInfo MUST be static here.");

            CodeGenerator gen = new CodeGenerator(targetModule);
            gen.BeginMethod("dm" + Guid.NewGuid().ToString("N"), typeof(StaticDynamicMethodProxyHandler));
            gen.LoadMember(fieldInfo);
            CastValueToObject(gen, fieldInfo.FieldType);

            return (StaticDynamicMethodProxyHandler)gen.EndMethod();
        }

        /// <summary>
        /// Do Get Static Field Set Delegate
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected static StaticDynamicMethodProxyHandler DoGetStaticFieldSetDelegate(
            Module targetModule,
            FieldInfo fieldInfo
            )
        {
            Check.Require(targetModule, "targetModule");
            Check.Require(fieldInfo, "fieldInfo");
            Check.Require(fieldInfo.IsStatic, "fieldInfo MUST be static here.");

            CodeGenerator gen = new CodeGenerator(targetModule);
            gen.BeginMethod("dm" + Guid.NewGuid().ToString("N"), typeof(StaticDynamicMethodProxyHandler));
            gen.Ldarg(0);
            gen.Ldc(0);
            gen.Ldelem(typeof(object));
            if (fieldInfo.FieldType.IsValueType)
                gen.UnboxAny(fieldInfo.FieldType);
            gen.StoreMember(fieldInfo);
            gen.Load(null);

            return (StaticDynamicMethodProxyHandler)gen.EndMethod();
        }

        /// <summary>
        /// Get delegate of a static method
        /// </summary>
        /// <param name="genericMethodInfo"></param>
        /// <param name="genericParameterTypes"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticMethodDelegate(
            MethodInfo genericMethodInfo, 
            params Type[] genericParameterTypes)
        {
            return DoGetStaticMethodDelegate(typeof(string).Module, genericMethodInfo, genericParameterTypes);
        }

        /// <summary>
        /// Get delegate of a static method defined in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="genericMethodInfo"></param>
        /// <param name="genericParameterTypes"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticMethodDelegate(
            Module targetModule, 
            MethodInfo genericMethodInfo, 
            params Type[] genericParameterTypes)
        {
            return DoGetStaticMethodDelegate(targetModule, genericMethodInfo, genericParameterTypes);
        }

        /// <summary>
        /// Get StaticPropertyGetMethod Delegate
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticPropertyGetMethodDelegate(
            PropertyInfo propertyInfo
            )
        {
            Check.Require(propertyInfo, "propertyInfo");

            return GetStaticMethodDelegate(propertyInfo.GetGetMethod());
        }

        /// <summary>
        /// Get StaticPropertyGetMethod Delegate defined in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticPropertyGetMethodDelegate(
            Module targetModule,
            PropertyInfo propertyInfo
            )
        {
            Check.Require(targetModule, "targetModule");
            Check.Require(propertyInfo, "propertyInfo");

            return GetStaticMethodDelegate(targetModule, propertyInfo.GetGetMethod());
        }

        /// <summary>
        /// Get StaticPropertySetMethod Delegate
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticPropertySetMethodDelegate(
            PropertyInfo propertyInfo
            )
        {
            Check.Require(propertyInfo, "propertyInfo");

            return GetStaticMethodDelegate(propertyInfo.GetSetMethod());
        }

        /// <summary>
        /// Get StaticPropertySetMethod Delegate defined in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticPropertySetMethodDelegate(
            Module targetModule,
            PropertyInfo propertyInfo
            )
        {
            Check.Require(targetModule, "targetModule");
            Check.Require(propertyInfo, "propertyInfo");

            return GetStaticMethodDelegate(targetModule, propertyInfo.GetSetMethod());
        }

        /// <summary>
        /// Get Static Field Get Delegate
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticFieldGetDelegate(
            FieldInfo fieldInfo
            )
        {
            return DoGetStaticFieldGetDelegate(typeof(string).Module, fieldInfo);
        }

        /// <summary>
        /// Get Static Field Get Delegate in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticFieldGetDelegate(
            Module targetModule,
            FieldInfo fieldInfo
            )
        {
            return DoGetStaticFieldGetDelegate(targetModule, fieldInfo);
        }

        /// <summary>
        /// Get Static Field Set Delegate
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticFieldSetDelegate(
            FieldInfo fieldInfo
            )
        {
            return DoGetStaticFieldSetDelegate(typeof(string).Module, fieldInfo);
        }

        /// <summary>
        /// Get Static Field Set Delegate in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public virtual StaticDynamicMethodProxyHandler GetStaticFieldSetDelegate(
            Module targetModule,
            FieldInfo fieldInfo
            )
        {
            return DoGetStaticFieldSetDelegate(targetModule, fieldInfo);
        }

        #endregion

        #region Get non-static delegate

        /// <summary>
        /// Do GetMethodDelegate
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="genericMethodInfo"></param>
        /// <param name="genericParameterTypes"></param>
        /// <returns></returns>
        protected static DynamicMethodProxyHandler DoGetMethodDelegate(
            Module targetModule,
            MethodInfo genericMethodInfo,
            params Type[] genericParameterTypes)
        {
            #region Check preconditions

            Check.Require(targetModule, "targetModule");
            Check.Require(genericMethodInfo, "genericMethodInfo");
            Check.Require((genericParameterTypes == null && genericMethodInfo.GetGenericArguments().Length == 0) ||
                genericParameterTypes.Length == genericMethodInfo.GetGenericArguments().Length,
                "The number of generic type parameter of genericMethodInfo and the input types must equal!");
            Check.Require(!genericMethodInfo.IsStatic, "genericMethodInfo must not be static here!");

            #endregion

            //Create a dynamic method proxy delegate used to call the specified methodinfo
            CodeGenerator gen = new CodeGenerator(targetModule);
            gen.BeginMethod("dm" + Guid.NewGuid().ToString("N"), typeof(DynamicMethodProxyHandler));
            MethodInfo makeGenericMethodInfo = MakeMethodGeneric(genericMethodInfo, genericParameterTypes);
            gen.Ldarg(0);
            LoadParameters(gen, makeGenericMethodInfo.GetParameters(), false);
            gen.Call(makeGenericMethodInfo);
            CastValueToObject(gen, makeGenericMethodInfo.ReturnType);

            return (DynamicMethodProxyHandler)gen.EndMethod();
        }

        /// <summary>
        /// Do Get Field Get Delegate
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected static DynamicMethodProxyHandler DoGetFieldGetDelegate(
            Module targetModule,
            FieldInfo fieldInfo
            )
        {
            Check.Require(targetModule, "targetModule");
            Check.Require(fieldInfo, "fieldInfo");
            Check.Require(!fieldInfo.IsStatic, "fieldInfo could not be static.");

            CodeGenerator gen = new CodeGenerator(targetModule);
            gen.BeginMethod("dm" + Guid.NewGuid().ToString("N"), typeof(DynamicMethodProxyHandler));
            gen.Ldarg(0);
            gen.LoadMember(fieldInfo);
            CastValueToObject(gen, fieldInfo.FieldType);

            return (DynamicMethodProxyHandler)gen.EndMethod();
        }

        /// <summary>
        /// Do Get Field Set Delegate
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        protected static DynamicMethodProxyHandler DoGetFieldSetDelegate(
            Module targetModule,
            FieldInfo fieldInfo
            )
        {
            Check.Require(targetModule, "targetModule");
            Check.Require(fieldInfo, "fieldInfo");
            Check.Require(!fieldInfo.IsStatic, "fieldInfo could not be static.");

            CodeGenerator gen = new CodeGenerator(targetModule);
            gen.BeginMethod("dm" + Guid.NewGuid().ToString("N"), typeof(DynamicMethodProxyHandler));
            gen.Ldarg(0);
            gen.Ldarg(1);
            gen.Ldc(0);
            gen.Ldelem(typeof(object));
            if (fieldInfo.FieldType.IsValueType)
                gen.UnboxAny(fieldInfo.FieldType);
            gen.StoreMember(fieldInfo);
            gen.Load(null);

            return (DynamicMethodProxyHandler)gen.EndMethod();
        }

        /// <summary>
        /// Get delegate of a static method
        /// </summary>
        /// <param name="genericMethodInfo"></param>
        /// <param name="genericParameterTypes"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetMethodDelegate(
            MethodInfo genericMethodInfo,
            params Type[] genericParameterTypes)
        {
            return DoGetMethodDelegate(typeof(string).Module, genericMethodInfo, genericParameterTypes);
        }

        /// <summary>
        /// Get delegate of a static method defined in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="genericMethodInfo"></param>
        /// <param name="genericParameterTypes"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetMethodDelegate(
            Module targetModule,
            MethodInfo genericMethodInfo,
            params Type[] genericParameterTypes)
        {
            return DoGetMethodDelegate(targetModule, genericMethodInfo, genericParameterTypes);
        }

        /// <summary>
        /// Get PropertyGetMethod Delegate
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetPropertyGetMethodDelegate(
            PropertyInfo propertyInfo
            )
        {
            Check.Require(propertyInfo, "propertyInfo");

            return GetMethodDelegate(propertyInfo.GetGetMethod());
        }

        /// <summary>
        /// Get PropertyGetMethod Delegate defined in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetPropertyGetMethodDelegate(
            Module targetModule,
            PropertyInfo propertyInfo
            )
        {
            Check.Require(targetModule, "targetModule");
            Check.Require(propertyInfo, "propertyInfo");

            return GetMethodDelegate(targetModule, propertyInfo.GetGetMethod());
        }

        /// <summary>
        /// Get PropertySetMethod Delegate
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetPropertySetMethodDelegate(
            PropertyInfo propertyInfo
            )
        {
            Check.Require(propertyInfo, "propertyInfo");

            return GetMethodDelegate(propertyInfo.GetSetMethod());
        }

        /// <summary>
        /// Get PropertySetMethod Delegate defined in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetPropertySetMethodDelegate(
            Module targetModule,
            PropertyInfo propertyInfo
            )
        {
            Check.Require(targetModule, "targetModule");
            Check.Require(propertyInfo, "propertyInfo");

            return GetMethodDelegate(targetModule, propertyInfo.GetSetMethod());
        }

        /// <summary>
        /// Get Field Get Delegate
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetFieldGetDelegate(
            FieldInfo fieldInfo
            )
        {
            return DoGetFieldGetDelegate(typeof(string).Module, fieldInfo);
        }

        /// <summary>
        /// Get Field Get Delegate in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetFieldGetDelegate(
            Module targetModule,
            FieldInfo fieldInfo
            )
        {
            return DoGetFieldGetDelegate(targetModule, fieldInfo);
        }

        /// <summary>
        /// Get Field Set Delegate
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetFieldSetDelegate(
            FieldInfo fieldInfo
            )
        {
            return DoGetFieldSetDelegate(typeof(string).Module, fieldInfo);
        }

        /// <summary>
        /// Get Field Set Delegate in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public virtual DynamicMethodProxyHandler GetFieldSetDelegate(
            Module targetModule,
            FieldInfo fieldInfo
            )
        {
            return DoGetFieldSetDelegate(targetModule, fieldInfo);
        }

        #endregion

        #region Create internal class instance

        /// <summary>
        /// Create internal class instance in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="typeFullName"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="isPublic"></param>
        /// <param name="binder"></param>
        /// <param name="culture"></param>
        /// <param name="activationAttrs"></param>
        /// <param name="paramObjs"></param>
        /// <returns></returns>
        public object CreateInstance(Module targetModule, string typeFullName, bool ignoreCase, bool isPublic, Binder binder, System.Globalization.CultureInfo culture, object[] activationAttrs, params object[] paramObjs)
        {
            if (targetModule == null)
            {
                throw new ArgumentNullException("targetModule");
            }

            //get method info of Assembly.CreateInstance() method first
            MethodInfo mi = ReflectionUtils.GetMethodInfoFromArrayBySignature(
                "System.Object CreateInstance(System.String, Boolean, System.Reflection.BindingFlags, System.Reflection.Binder, System.Object[], System.Globalization.CultureInfo, System.Object[])",
                typeof(Assembly).GetMethods());

            DynamicMethodProxyHandler dmd = GetMethodDelegate(targetModule, mi);
            return dmd(targetModule.Assembly, new object[] { typeFullName, ignoreCase, BindingFlags.Instance | (isPublic ? BindingFlags.Public : BindingFlags.NonPublic), binder, paramObjs, culture, activationAttrs });
        }

        /// <summary>
        /// Create internal class instance in specific Module
        /// </summary>
        /// <param name="targetModule"></param>
        /// <param name="typeFullName"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="isPublic"></param>
        /// <param name="paramObjs"></param>
        /// <returns></returns>
        public object CreateInstance(Module targetModule, string typeFullName, bool ignoreCase, bool isPublic, params object[] paramObjs)
        {
            return CreateInstance(targetModule, typeFullName, ignoreCase, isPublic, null, null, null, paramObjs);
        }

        #endregion
    }

    #region UnitTest

#if DEBUG

    namespace DynamicMethodFactoryUnitTest
    {
        public class TestClass
        {
            public static void StaticReturnVoidMethod()
            {
            }

            public static int StaticReturnIntMethod(string str, int i, ref int refInt, ref string refStr)
            {
                Check.Assert(str == "str");
                Check.Assert(i == 1);
                Check.Assert(refInt == 3);
                Check.Assert(refStr == "instr");

                int ret = i + refInt;
                refInt = i + 1;
                refStr = "ref" + str;

                Check.Assert(refInt == 2);
                Check.Assert(ret == 4);
                Check.Assert(refStr == "refstr");

                return ret;
            }

            public static int StaticIntField;

            public static int StaticIntProperty
            {
                get
                {
                    return StaticIntField;
                }
                set
                {
                    StaticIntField = value;
                }
            }

            public void NonStaticReturnVoidMethod()
            {
            }

            public int NonStaticReturnIntMethod(string str, int i, out int outInt, out string outStr)
            {
                outInt = i + 1;
                Check.Assert(outInt == 2);
                outStr = "out" + str;
                Check.Assert(outStr == "outstr");
                return i + 2;
            }

            public int NonStaticIntField;

            public int NonStaticIntProperty
            {
                get
                {
                    return NonStaticIntField;
                }
                set
                {
                    NonStaticIntField = value;
                }
            }
        }

        public class UnitTest
        {
            private static DynamicMethodFactory fac = new DynamicMethodFactory();

            public static void TestStaticMethod()
            {
                StaticDynamicMethodProxyHandler handler = fac.GetStaticMethodDelegate(typeof(TestClass).GetMethod("StaticReturnVoidMethod"));
                handler(null);

                object[] inputParams = new object[] { "str", 1, 3, "instr" };
                handler = fac.GetStaticMethodDelegate(typeof(TestClass).GetMethod("StaticReturnIntMethod"));
                object ret = handler(inputParams);
                Check.Assert(((int)inputParams[2]) == 2);
                Check.Assert(((string)inputParams[3]) == "refstr");
                Check.Assert(((int)ret) == 4);
            }

            public static void TestStaticField()
            {
                TestClass.StaticIntField = -1;
                FieldInfo field = typeof(TestClass).GetField("StaticIntField"); ;
                StaticDynamicMethodProxyHandler handler = fac.GetStaticFieldSetDelegate(field);
                handler(new object[] { 5 });
                Check.Assert(TestClass.StaticIntField == 5);
                handler = fac.GetStaticFieldGetDelegate(field);
                Check.Assert(((int)handler(null)) == 5);
            }

            public static void TestStaticProperty()
            {
                TestClass.StaticIntField = -1;
                PropertyInfo property = typeof(TestClass).GetProperty("StaticIntProperty"); ;
                StaticDynamicMethodProxyHandler handler = fac.GetStaticMethodDelegate(property.GetSetMethod());
                handler(new object[] { 5 });
                Check.Assert(TestClass.StaticIntProperty == 5);
                handler = fac.GetStaticMethodDelegate(property.GetGetMethod());
                Check.Assert(((int)handler(null)) == 5);
            }

            public static void TestNonStaticMethod()
            {
                TestClass obj = new TestClass();

                DynamicMethodProxyHandler handler = fac.GetMethodDelegate(typeof(TestClass).GetMethod("NonStaticReturnVoidMethod"));
                handler(obj, null);

                object[] inputParams = new object[] { "str", 1, null, null };
                handler = fac.GetMethodDelegate(typeof(TestClass).GetMethod("NonStaticReturnIntMethod"));
                object ret = handler(obj, inputParams);
                Check.Assert(((int)inputParams[2]) == 2);
                Check.Assert(((string)inputParams[3]) == "outstr");
                Check.Assert(((int)ret) == 3);
            }

            public static void TestNonStaticField()
            {
                TestClass obj = new TestClass();
                obj.NonStaticIntField = -1;

                FieldInfo field = typeof(TestClass).GetField("NonStaticIntField"); ;
                DynamicMethodProxyHandler handler = fac.GetFieldSetDelegate(field);
                handler(obj, new object[] { 5 });
                Check.Assert(obj.NonStaticIntField == 5);
                handler = fac.GetFieldGetDelegate(field);
                Check.Assert(((int)handler(obj, null)) == 5);
            }

            public static void TestNonStaticProperty()
            {
                TestClass obj = new TestClass();
                obj.NonStaticIntField = -1;

                PropertyInfo property = typeof(TestClass).GetProperty("NonStaticIntProperty"); ;
                DynamicMethodProxyHandler handler = fac.GetMethodDelegate(property.GetSetMethod());
                handler(obj, new object[] { 5 });
                Check.Assert(obj.NonStaticIntField == 5);
                handler = fac.GetMethodDelegate(property.GetGetMethod());
                Check.Assert(((int)handler(obj, null)) == 5);
            }
        }
    }

#endif

    #endregion
}
