//IL指令的含义可以通过MSDN得到解释：如：http://msdn2.microsoft.com/zh-cn/library/system.reflection.emit.opcodes.Ldobj.aspx
//如果是其它指令，可以替换Ldobj：比如：ldloca  http://msdn2.microsoft.com/zh-cn/library/system.reflection.emit.opcodes.ldloca.aspx
//或ldloca.s 将 http://msdn2.microsoft.com/zh-cn/library/system.reflection.emit.opcodes.ldloca_s.aspx
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace System.Reflection.Emit
{
    /// <summary>
    /// Cmp
    /// </summary>
    public enum Cmp
    {
        /// <summary>
        /// LessThan
        /// </summary>
        LessThan,
        /// <summary>
        /// EqualTo
        /// </summary>
        EqualTo,
        /// <summary>
        /// LessThanOrEqualTo
        /// </summary>
        LessThanOrEqualTo,
        /// <summary>
        /// GreaterThan
        /// </summary>
        GreaterThan,
        /// <summary>
        /// NotEqualTo
        /// </summary>
        NotEqualTo,
        /// <summary>
        /// GreaterThanOrEqualTo
        /// </summary>
        GreaterThanOrEqualTo
    }

    /// <summary>
    /// ArgBuilder
    /// </summary>
    public class ArgBuilder
    {
        /// <summary>
        /// ArgType
        /// </summary>
        public Type ArgType { get; set; }
        /// <summary>
        /// Index
        /// </summary>
        
        public int Index { get; set; }

        /// <summary>
        /// ArgBuilder
        /// </summary>
        /// <param name="index"></param>
        /// <param name="argType"></param>
        public ArgBuilder(int index, Type argType)
        {
            this.Index = index;
            this.ArgType = argType;
        }
    }

    /// <summary>
    /// IfState
    /// </summary>
    public class IfState
    {
        // Fields
        private Label elseBegin;
        private Label endIf;

        /// <summary>
        /// ElseBegin
        /// </summary>
        public Label ElseBegin
        {
            get
            {
                return this.elseBegin;
            }
            set
            {
                this.elseBegin = value;
            }
        }

        /// <summary>
        /// EndIf
        /// </summary>
        public Label EndIf
        {
            get
            {
                return this.endIf;
            }
            set
            {
                this.endIf = value;
            }
        }
    }

    /// <summary>
    /// SwitchState
    /// </summary>
    public class SwitchState
    {
        private bool defaultDefined;
        private Label defaultLabel;
        private Label endOfSwitchLabel;

        /// <summary>
        /// SwitchState
        /// </summary>
        /// <param name="defaultLabel"></param>
        /// <param name="endOfSwitchLabel"></param>
        public SwitchState(Label defaultLabel, Label endOfSwitchLabel)
        {
            this.defaultLabel = defaultLabel;
            this.endOfSwitchLabel = endOfSwitchLabel;
            this.defaultDefined = false;
        }

        /// <summary>
        /// DefaultDefined
        /// </summary>
        public bool DefaultDefined
        {
            get
            {
                return this.defaultDefined;
            }
            set
            {
                this.defaultDefined = value;
            }
        }

        /// <summary>
        /// DefaultLabel
        /// </summary>
        public Label DefaultLabel
        {
            get
            {
                return this.defaultLabel;
            }
        }

        /// <summary>
        /// EndOfSwitchLabel
        /// </summary>
        public Label EndOfSwitchLabel
        {
            get
            {
                return this.endOfSwitchLabel;
            }
        }
    }

    /// <summary>
    /// ForState
    /// </summary>
    public class ForState
    {
        // Fields
        private Label beginLabel;
        private object end;
        private Label endLabel;
        private LocalBuilder indexVar;
        private bool requiresEndLabel;
        private Label testLabel;

        /// <summary>
        /// ForState
        /// </summary>
        /// <param name="indexVar"></param>
        /// <param name="beginLabel"></param>
        /// <param name="testLabel"></param>
        /// <param name="end"></param>
        public ForState(LocalBuilder indexVar, Label beginLabel, Label testLabel, object end)
        {
            this.indexVar = indexVar;
            this.beginLabel = beginLabel;
            this.testLabel = testLabel;
            this.end = end;
        }

        /// <summary>
        /// BeginLabel
        /// </summary>
        public Label BeginLabel
        {
            get
            {
                return this.beginLabel;
            }
        }

        /// <summary>
        /// End
        /// </summary>
        public object End
        {
            get
            {
                return this.end;
            }
        }

        /// <summary>
        /// EndLabel
        /// </summary>
        public Label EndLabel
        {
            get
            {
                return this.endLabel;
            }
            set
            {
                this.endLabel = value;
            }
        }

        /// <summary>
        /// Index
        /// </summary>
        public LocalBuilder Index
        {
            get
            {
                return this.indexVar;
            }
        }

        /// <summary>
        /// RequiresEndLabel
        /// </summary>
        public bool RequiresEndLabel
        {
            get
            {
                return this.requiresEndLabel;
            }
            set
            {
                this.requiresEndLabel = value;
            }
        }

        /// <summary>
        /// TestLabel
        /// </summary>
        public Label TestLabel
        {
            get
            {
                return this.testLabel;
            }
        }
    }

    /// <summary>
    /// CodeGenerator
    /// </summary>
    public class CodeGenerator
    {
        private ArrayList argList;
        private Stack blockStack;
        private Type delegateType;
        private DynamicMethod dynamicMethod;
        private MethodBase methodOrConstructorBuilder;
        private static MethodInfo getTypeFromHandle;
        private ILGenerator ilGen;
        private Hashtable localNames;
        private Label methodEndLabel;
        private static MethodInfo objectEquals;
        private static MethodInfo objectToString;
        private Module serializationModule = typeof(CodeGenerator).Module;
        private static MethodInfo stringConcat2;
        private static MethodInfo stringConcat3;
        private static MethodInfo stringFormat;
        private LocalBuilder stringFormatArray;

        /// <summary>
        /// Initialize a CodeGenerator instance
        /// </summary>
        public CodeGenerator()
        {
        }

        /// <summary>
        /// Initialize a CodeGenerator instance
        /// </summary>
        /// <param name="targetModule"></param>
        public CodeGenerator(Module targetModule)
            : this()
        {
            Check.Require(targetModule, "targetModule");

            this.serializationModule = targetModule;
        }

        /// <summary>
        /// Initialize a CodeGenerator instance
        /// </summary>
        /// <param name="ownerTypeBuilder"></param>
        /// <param name="methodName"></param>
        /// <param name="methodAttrs"></param>
        /// <param name="callingConversion"></param>
        /// <param name="returnType"></param>
        /// <param name="argTypes"></param>
        public CodeGenerator(TypeBuilder ownerTypeBuilder, string methodName, MethodAttributes methodAttrs,
            CallingConventions callingConversion, Type returnType, Type[] argTypes)
            : this()
        {
            Check.Require(ownerTypeBuilder, "ownerTypeBuilder");
            Check.Require(methodName, "methodName", Check.NotNullOrEmpty);

            if (methodName == "ctor") //constructor
            {
                this.methodOrConstructorBuilder = ownerTypeBuilder.DefineConstructor(
                    methodAttrs, callingConversion, argTypes);
                this.ilGen = (this.methodOrConstructorBuilder as ConstructorBuilder).GetILGenerator();
            }
            else // method
            {
                this.methodOrConstructorBuilder = ownerTypeBuilder.DefineMethod(methodName,
                    methodAttrs, callingConversion, returnType, argTypes);
                this.ilGen = (this.methodOrConstructorBuilder as MethodBuilder).GetILGenerator();
            }
            this.blockStack = new Stack();
            this.argList = new ArrayList();
            if (argTypes != null)
            {
                for (int i = 0; i < argTypes.Length; i++)
                {
                    this.argList.Add(new ArgBuilder(i, argTypes[i]));
                }
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        public void Add()
        {
            this.ilGen.Emit(OpCodes.Add);
        }

        /// <summary>
        /// And
        /// </summary>
        public void And()
        {
            this.ilGen.Emit(OpCodes.And);
        }

        /// <summary>
        /// BeginMethod
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="delegateType"></param>
        public void BeginMethod(string methodName, Type delegateType)
        {
            Check.Require(this.methodOrConstructorBuilder == null, "BeginMethod() could not be called in this context.");
            Check.Require(methodName, "methodName", Check.NotNullOrEmpty);
            Check.Require(delegateType, "delegateType");

            MethodInfo method = delegateType.GetMethod("Invoke");
            ParameterInfo[] parameters = method.GetParameters();
            Type[] argTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                argTypes[i] = parameters[i].ParameterType;
            }
            this.BeginMethod(method.ReturnType, methodName, argTypes);
            this.delegateType = delegateType;
        }

        /// <summary>
        /// BeginMethod
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="methodName"></param>
        /// <param name="argTypes"></param>
        private void BeginMethod(Type returnType, string methodName, params Type[] argTypes)
        {
            this.dynamicMethod = new DynamicMethod(methodName, returnType, argTypes, serializationModule, true);
            this.ilGen = this.dynamicMethod.GetILGenerator();
            this.methodEndLabel = this.ilGen.DefineLabel();
            this.blockStack = new Stack();
            this.argList = new ArrayList();
            for (int i = 0; i < argTypes.Length; i++)
            {
                this.argList.Add(new ArgBuilder(i, argTypes[i]));
            }
        }

        /// <summary>
        /// Bgt
        /// </summary>
        /// <param name="label"></param>
        public void Bgt(Label label)
        {
            this.ilGen.Emit(OpCodes.Bgt, label);
        }

        /// <summary>
        /// Ble
        /// </summary>
        /// <param name="label"></param>
        public void Ble(Label label)
        {
            this.ilGen.Emit(OpCodes.Ble, label);
        }

        /// <summary>
        /// Blt
        /// </summary>
        /// <param name="label"></param>
        public void Blt(Label label)
        {
            this.ilGen.Emit(OpCodes.Blt, label);
        }

        /// <summary>
        /// Box
        /// </summary>
        /// <param name="type"></param>
        public void Box(Type type)
        {
            Check.Require(type, "type");
            Check.Require(type.IsValueType, "type MUST be ValueType");

            this.ilGen.Emit(OpCodes.Box, type);
        }

        /// <summary>
        /// Br 无条件地将控制转移到目标指令
        /// </summary>
        /// <param name="label"></param>
        public void Br(Label label)
        {
            this.ilGen.Emit(OpCodes.Br, label);
        }

        /// <summary>
        /// Break
        /// </summary>
        /// <param name="forState"></param>
        public void Break(object forState)
        {
            this.InternalBreakFor(forState, OpCodes.Br);
        }

        /// <summary>
        /// Brfalse
        /// </summary>
        /// <param name="label"></param>
        public void Brfalse(Label label)
        {
            this.ilGen.Emit(OpCodes.Brfalse, label);
        }

        /// <summary>
        /// Brtrue
        /// </summary>
        /// <param name="label"></param>
        public void Brtrue(Label label)
        {
            this.ilGen.Emit(OpCodes.Brtrue, label);
        }

        /// <summary>
        /// Call 调用由传递的方法说明符指示的方法。
        /// </summary>
        /// <param name="ctor"></param>
        public void Call(ConstructorInfo ctor)
        {
            Check.Require(ctor, "ctor");

            this.ilGen.Emit(OpCodes.Call, ctor);
        }

        /// <summary>
        /// Call 调用由传递的方法说明符指示的方法。
        /// </summary>
        /// <param name="methodInfo"></param>
        public void Call(MethodInfo methodInfo)
        {
            Check.Require(methodInfo, "methodInfo");

            if (methodInfo.IsVirtual)
            {
                this.ilGen.Emit(OpCodes.Callvirt, methodInfo);
            }
            else if (methodInfo.IsStatic)
            {
                this.ilGen.Emit(OpCodes.Call, methodInfo);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Call, methodInfo);
            }
        }

        /// <summary>
        /// Call
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="methodInfo"></param>
        public void Call(object thisObj, MethodInfo methodInfo)
        {
            Check.Require(thisObj, "thisObj");
            Check.Require(methodInfo, "methodInfo");

            this.VerifyParameterCount(methodInfo, 0);
            this.LoadThis(thisObj, methodInfo);
            this.Call(methodInfo);
        }

        /// <summary>
        /// Call
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="methodInfo"></param>
        /// <param name="param1"></param>
        public void Call(object thisObj, MethodInfo methodInfo, object param1)
        {
            Check.Require(thisObj, "thisObj");
            Check.Require(methodInfo, "methodInfo");

            this.VerifyParameterCount(methodInfo, 1);
            this.LoadThis(thisObj, methodInfo);
            this.LoadParam(param1, 1, methodInfo);
            this.Call(methodInfo);
        }

        /// <summary>
        /// Call
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="methodInfo"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public void Call(object thisObj, MethodInfo methodInfo, object param1, object param2)
        {
            Check.Require(thisObj, "thisObj");
            Check.Require(methodInfo, "methodInfo");

            this.VerifyParameterCount(methodInfo, 2);
            this.LoadThis(thisObj, methodInfo);
            this.LoadParam(param1, 1, methodInfo);
            this.LoadParam(param2, 2, methodInfo);
            this.Call(methodInfo);
        }

        /// <summary>
        /// Call
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="methodInfo"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        public void Call(object thisObj, MethodInfo methodInfo, object param1, object param2, object param3)
        {
            Check.Require(thisObj, "thisObj");
            Check.Require(methodInfo, "methodInfo");

            this.VerifyParameterCount(methodInfo, 3);
            this.LoadThis(thisObj, methodInfo);
            this.LoadParam(param1, 1, methodInfo);
            this.LoadParam(param2, 2, methodInfo);
            this.LoadParam(param3, 3, methodInfo);
            this.Call(methodInfo);
        }

        /// <summary>
        /// Call
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="methodInfo"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="param4"></param>
        public void Call(object thisObj, MethodInfo methodInfo, object param1, object param2, object param3, object param4)
        {
            Check.Require(thisObj, "thisObj");
            Check.Require(methodInfo, "methodInfo");

            this.VerifyParameterCount(methodInfo, 4);
            this.LoadThis(thisObj, methodInfo);
            this.LoadParam(param1, 1, methodInfo);
            this.LoadParam(param2, 2, methodInfo);
            this.LoadParam(param3, 3, methodInfo);
            this.LoadParam(param4, 4, methodInfo);
            this.Call(methodInfo);
        }

        /// <summary>
        /// Call
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="methodInfo"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="param4"></param>
        /// <param name="param5"></param>
        public void Call(object thisObj, MethodInfo methodInfo, object param1, object param2, object param3, object param4, object param5)
        {
            Check.Require(thisObj, "thisObj");
            Check.Require(methodInfo, "methodInfo");

            this.VerifyParameterCount(methodInfo, 5);
            this.LoadThis(thisObj, methodInfo);
            this.LoadParam(param1, 1, methodInfo);
            this.LoadParam(param2, 2, methodInfo);
            this.LoadParam(param3, 3, methodInfo);
            this.LoadParam(param4, 4, methodInfo);
            this.LoadParam(param5, 5, methodInfo);
            this.Call(methodInfo);
        }

        /// <summary>
        /// Call
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="methodInfo"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="param4"></param>
        /// <param name="param5"></param>
        /// <param name="param6"></param>
        public void Call(object thisObj, MethodInfo methodInfo, object param1, object param2, object param3, object param4, object param5, object param6)
        {
            Check.Require(thisObj, "thisObj");
            Check.Require(methodInfo, "methodInfo");

            this.VerifyParameterCount(methodInfo, 6);
            this.LoadThis(thisObj, methodInfo);
            this.LoadParam(param1, 1, methodInfo);
            this.LoadParam(param2, 2, methodInfo);
            this.LoadParam(param3, 3, methodInfo);
            this.LoadParam(param4, 4, methodInfo);
            this.LoadParam(param5, 5, methodInfo);
            this.LoadParam(param6, 6, methodInfo);
            this.Call(methodInfo);
        }

        /// <summary>
        /// CallStringFormat
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="values"></param>
        public void CallStringFormat(string msg, params object[] values)
        {
            Check.Require(msg, "msg", Check.NotNullOrEmpty);

            this.NewArray(typeof(object), values.Length);
            if (this.stringFormatArray == null)
            {
                this.stringFormatArray = this.DeclareLocal(typeof(object[]), "stringFormatArray");
            }
            this.Stloc(this.stringFormatArray);
            for (int i = 0; i < values.Length; i++)
            {
                this.StoreArrayElement(this.stringFormatArray, i, values[i]);
            }
            this.Load(msg);
            this.Load(this.stringFormatArray);
            this.Call(StringFormat);
        }

        /// <summary>
        /// Case
        /// </summary>
        /// <param name="caseLabel1"></param>
        /// <param name="caseLabelName"></param>
        public void Case(Label caseLabel1, string caseLabelName)
        {
            this.MarkLabel(caseLabel1);
        }

        /// <summary>
        /// Castclass
        /// </summary>
        /// <param name="target"></param>
        public void Castclass(Type target)
        {
            Check.Require(target, "target");

            this.ilGen.Emit(OpCodes.Castclass, target);
        }

        /// <summary>
        /// Ceq
        /// </summary>
        public void Ceq()
        {
            this.ilGen.Emit(OpCodes.Ceq);
        }

        /// <summary>
        /// Concat2
        /// </summary>
        public void Concat2()
        {
            this.Call(StringConcat2);
        }

        /// <summary>
        /// Concat3
        /// </summary>
        public void Concat3()
        {
            this.Call(StringConcat3);
        }

        /// <summary>
        /// ConvertAddress
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void ConvertAddress(Type source, Type target)
        {
            Check.Require(source, "source");
            Check.Require(target, "target");

            this.InternalConvert(source, target, true);
        }

        /// <summary>
        /// ConvertValue
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void ConvertValue(Type source, Type target)
        {
            Check.Require(source, "source");
            Check.Require(target, "target");

            this.InternalConvert(source, target, false);
        }

        /// <summary>
        /// Dec
        /// </summary>
        /// <param name="var"></param>
        public void Dec(object var)
        {
            this.Load(var);
            this.Load(1);
            this.Subtract();
            this.Store(var);
        }

        /// <summary>
        /// DeclareLocal
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public LocalBuilder DeclareLocal(Type type)
        {
            Check.Require(type, "type");

            return this.DeclareLocal(type, null, false);
        }

        /// <summary>
        /// DeclareLocal
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public LocalBuilder DeclareLocal(Type type, string name)
        {
            Check.Require(type, "type");

            return this.DeclareLocal(type, name, false);
        }

        /// <summary>
        /// DeclareLocal
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="isPinned"></param>
        /// <returns></returns>
        public LocalBuilder DeclareLocal(Type type, string name, bool isPinned)
        {
            Check.Require(type, "type");

            LocalBuilder builder = this.ilGen.DeclareLocal(type, isPinned);
            return builder;
        }

        /// <summary>
        /// DeclareLocal
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        public LocalBuilder DeclareLocal(Type type, string name, object initialValue)
        {
            Check.Require(type, "type");

            LocalBuilder var = this.DeclareLocal(type, name);
            this.Load(initialValue);
            this.Store(var);
            return var;
        }

        /// <summary>
        /// DefaultCase
        /// </summary>
        public void DefaultCase()
        {
            object expected = this.blockStack.Peek();
            SwitchState state = expected as SwitchState;
            if (state == null)
            {
                this.ThrowMismatchException(expected);
            }
            this.MarkLabel(state.DefaultLabel);
            state.DefaultDefined = true;
        }

        /// <summary>
        /// DefineLabel
        /// </summary>
        /// <returns></returns>
        public Label DefineLabel()
        {
            return this.ilGen.DefineLabel();
        }

        /// <summary>
        /// Dup
        /// </summary>
        public void Dup()
        {
            this.ilGen.Emit(OpCodes.Dup);
        }

        /// <summary>
        /// Else
        /// </summary>
        public void Else()
        {
            IfState state = this.PopIfState();
            this.Br(state.EndIf);
            this.MarkLabel(state.ElseBegin);
            state.ElseBegin = state.EndIf;
            this.blockStack.Push(state);
        }

        /// <summary>
        /// ElseIf
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="cmpOp"></param>
        /// <param name="value2"></param>
        public void ElseIf(object value1, Cmp cmpOp, object value2)
        {
            IfState state = (IfState)this.blockStack.Pop();
            this.Br(state.EndIf);
            this.MarkLabel(state.ElseBegin);
            this.Load(value1);
            this.Load(value2);
            state.ElseBegin = this.DefineLabel();
            this.ilGen.Emit(this.GetBranchCode(cmpOp), state.ElseBegin);
            this.blockStack.Push(state);
        }

        /// <summary>
        /// EndCase
        /// </summary>
        public void EndCase()
        {
            object expected = this.blockStack.Peek();
            SwitchState state = expected as SwitchState;
            if (state == null)
            {
                this.ThrowMismatchException(expected);
            }
            this.Br(state.EndOfSwitchLabel);
        }

        /// <summary>
        /// EndFor
        /// </summary>
        public void EndFor()
        {
            object expected = this.blockStack.Pop();
            ForState state = expected as ForState;
            if (state == null)
            {
                this.ThrowMismatchException(expected);
            }
            if (state.Index != null)
            {
                this.Ldloc(state.Index);
                this.Ldc(1);
                this.Add();
                this.Stloc(state.Index);
                this.MarkLabel(state.TestLabel);
                this.Ldloc(state.Index);
                this.Load(state.End);
                if (this.GetVariableType(state.End).IsArray)
                {
                    this.Ldlen();
                }
                this.Blt(state.BeginLabel);
            }
            else
            {
                this.Br(state.BeginLabel);
            }
            if (state.RequiresEndLabel)
            {
                this.MarkLabel(state.EndLabel);
            }
        }

        /// <summary>
        /// EndForEach
        /// </summary>
        /// <param name="moveNextMethod"></param>
        public void EndForEach(MethodInfo moveNextMethod)
        {
            Check.Require(moveNextMethod, "moveNextMethod");

            object expected = this.blockStack.Pop();
            ForState state = expected as ForState;
            if (state == null)
            {
                this.ThrowMismatchException(expected);
            }
            this.MarkLabel(state.TestLabel);
            object var = state.End;
            if (this.GetVariableType(var) == moveNextMethod.DeclaringType)
            {
                this.LoadThis(var, moveNextMethod);
                this.ilGen.Emit(OpCodes.Call, moveNextMethod);
            }
            else
            {
                this.Call(var, moveNextMethod);
            }
            this.Brtrue(state.BeginLabel);
            if (state.RequiresEndLabel)
            {
                this.MarkLabel(state.EndLabel);
            }
        }

        /// <summary>
        /// EndIf
        /// </summary>
        public void EndIf()
        {
            IfState state = this.PopIfState();
            if (!state.ElseBegin.Equals(state.EndIf))
            {
                this.MarkLabel(state.ElseBegin);
            }
            this.MarkLabel(state.EndIf);
        }

        /// <summary>
        /// EndMethod
        /// </summary>
        /// <returns></returns>
        public Delegate EndMethod()
        {
            Check.Require(this.methodOrConstructorBuilder == null, "EndMethod() could not be called in this context.");

            this.MarkLabel(this.methodEndLabel);
            this.Ret();
            Delegate delegate2 = null;
            delegate2 = this.dynamicMethod.CreateDelegate(this.delegateType);
            this.dynamicMethod = null;
            this.delegateType = null;
            this.ilGen = null;
            this.blockStack = null;
            this.argList = null;
            return delegate2;
        }

        /// <summary>
        /// EndSwitch
        /// </summary>
        public void EndSwitch()
        {
            object expected = this.blockStack.Pop();
            SwitchState state = expected as SwitchState;
            if (state == null)
            {
                this.ThrowMismatchException(expected);
            }
            if (!state.DefaultDefined)
            {
                this.MarkLabel(state.DefaultLabel);
            }
            this.MarkLabel(state.EndOfSwitchLabel);
        }

        /// <summary>
        /// For
        /// </summary>
        /// <param name="local"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public object For(LocalBuilder local, object start, object end)
        {
            Check.Require(local, "local");

            ForState state = new ForState(local, this.DefineLabel(), this.DefineLabel(), end);
            if (state.Index != null)
            {
                this.Load(start);
                this.Stloc(state.Index);
                this.Br(state.TestLabel);
            }
            this.MarkLabel(state.BeginLabel);
            this.blockStack.Push(state);
            return state;
        }

        /// <summary>
        /// ForEach
        /// </summary>
        /// <param name="local"></param>
        /// <param name="elementType"></param>
        /// <param name="enumeratorType"></param>
        /// <param name="enumerator"></param>
        /// <param name="getCurrentMethod"></param>
        public void ForEach(LocalBuilder local, Type elementType, Type enumeratorType, LocalBuilder enumerator, MethodInfo getCurrentMethod)
        {
            Check.Require(local, "local");
            Check.Require(elementType, "elementType");
            Check.Require(enumeratorType, "enumeratorType");
            Check.Require(enumerator, "enumerator");
            Check.Require(getCurrentMethod, "getCurrentMethod");

            ForState state = new ForState(local, this.DefineLabel(), this.DefineLabel(), enumerator);
            this.Br(state.TestLabel);
            this.MarkLabel(state.BeginLabel);
            if (enumeratorType == getCurrentMethod.DeclaringType)
            {
                this.LoadThis(enumerator, getCurrentMethod);
                this.ilGen.Emit(OpCodes.Call, getCurrentMethod);
            }
            else
            {
                this.Call(enumerator, getCurrentMethod);
            }
            this.ConvertValue(elementType, this.GetVariableType(local));
            this.Stloc(local);
            this.blockStack.Push(state);
        }

        /// <summary>
        /// GetArg
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ArgBuilder GetArg(int index)
        {
            return (ArgBuilder)this.argList[index];
        }

        private OpCode GetBranchCode(Cmp cmp)
        {
            switch (cmp)
            {
                case Cmp.LessThan:
                    return OpCodes.Bge;

                case Cmp.EqualTo:
                    return OpCodes.Bne_Un;

                case Cmp.LessThanOrEqualTo:
                    return OpCodes.Bgt;

                case Cmp.GreaterThan:
                    return OpCodes.Ble;

                case Cmp.NotEqualTo:
                    return OpCodes.Beq;
            }
            return OpCodes.Blt;
        }

        private Cmp GetCmpInverse(Cmp cmp)
        {
            switch (cmp)
            {
                case Cmp.LessThan:
                    return Cmp.GreaterThanOrEqualTo;

                case Cmp.EqualTo:
                    return Cmp.NotEqualTo;

                case Cmp.LessThanOrEqualTo:
                    return Cmp.GreaterThan;

                case Cmp.GreaterThan:
                    return Cmp.LessThanOrEqualTo;

                case Cmp.NotEqualTo:
                    return Cmp.EqualTo;
            }
            return Cmp.LessThan;
        }

        private OpCode GetConvOpCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return OpCodes.Conv_I1;

                case TypeCode.Char:
                    return OpCodes.Conv_I2;

                case TypeCode.SByte:
                    return OpCodes.Conv_I1;

                case TypeCode.Byte:
                    return OpCodes.Conv_U1;

                case TypeCode.Int16:
                    return OpCodes.Conv_I2;

                case TypeCode.UInt16:
                    return OpCodes.Conv_U2;

                case TypeCode.Int32:
                    return OpCodes.Conv_I4;

                case TypeCode.UInt32:
                    return OpCodes.Conv_U4;

                case TypeCode.Int64:
                    return OpCodes.Conv_I8;

                case TypeCode.UInt64:
                    return OpCodes.Conv_I8;

                case TypeCode.Single:
                    return OpCodes.Conv_R4;

                case TypeCode.Double:
                    return OpCodes.Conv_R8;
            }
            return OpCodes.Nop;
        }

        private OpCode GetLdelemOpCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Object:
                case TypeCode.DBNull:
                    return OpCodes.Ldelem_Ref;

                case TypeCode.Boolean:
                    return OpCodes.Ldelem_I1;

                case TypeCode.Char:
                    return OpCodes.Ldelem_I2;

                case TypeCode.SByte:
                    return OpCodes.Ldelem_I1;

                case TypeCode.Byte:
                    return OpCodes.Ldelem_U1;

                case TypeCode.Int16:
                    return OpCodes.Ldelem_I2;

                case TypeCode.UInt16:
                    return OpCodes.Ldelem_U2;

                case TypeCode.Int32:
                    return OpCodes.Ldelem_I4;

                case TypeCode.UInt32:
                    return OpCodes.Ldelem_U4;

                case TypeCode.Int64:
                    return OpCodes.Ldelem_I8;

                case TypeCode.UInt64:
                    return OpCodes.Ldelem_I8;

                case TypeCode.Single:
                    return OpCodes.Ldelem_R4;

                case TypeCode.Double:
                    return OpCodes.Ldelem_R8;

                case TypeCode.String:
                    return OpCodes.Ldelem_Ref;
            }
            return OpCodes.Nop;
        }

        /// <summary>
        /// 根据值类型，返回使用哪个指令将值类型复制到计算堆栈上(ldobj指令的功能)
        /// 	<remark>abu 2007-10-16 11:49 AF043</remark>
        /// </summary>
        /// <param name="typeCode">The type code.</param>
        /// <returns></returns>
        private OpCode GetLdindOpCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return OpCodes.Ldind_I1;

                case TypeCode.Char:
                    return OpCodes.Ldind_I2;

                case TypeCode.SByte:
                    return OpCodes.Ldind_I1;

                case TypeCode.Byte:
                    return OpCodes.Ldind_U1;

                case TypeCode.Int16:
                    return OpCodes.Ldind_I2;

                case TypeCode.UInt16:
                    return OpCodes.Ldind_U2;

                case TypeCode.Int32:
                    return OpCodes.Ldind_I4;

                case TypeCode.UInt32:
                    return OpCodes.Ldind_U4;

                case TypeCode.Int64:
                    return OpCodes.Ldind_I8;

                case TypeCode.UInt64:
                    return OpCodes.Ldind_I8;

                case TypeCode.Single:
                    return OpCodes.Ldind_R4;

                case TypeCode.Double:
                    return OpCodes.Ldind_R8;

                case TypeCode.String:
                    return OpCodes.Ldind_Ref;
            }
            return OpCodes.Nop;
        }

        private OpCode GetStelemOpCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Object:
                case TypeCode.DBNull:
                    return OpCodes.Stelem_Ref;

                case TypeCode.Boolean:
                    return OpCodes.Stelem_I1;

                case TypeCode.Char:
                    return OpCodes.Stelem_I2;

                case TypeCode.SByte:
                    return OpCodes.Stelem_I1;

                case TypeCode.Byte:
                    return OpCodes.Stelem_I1;

                case TypeCode.Int16:
                    return OpCodes.Stelem_I2;

                case TypeCode.UInt16:
                    return OpCodes.Stelem_I2;

                case TypeCode.Int32:
                    return OpCodes.Stelem_I4;

                case TypeCode.UInt32:
                    return OpCodes.Stelem_I4;

                case TypeCode.Int64:
                    return OpCodes.Stelem_I8;

                case TypeCode.UInt64:
                    return OpCodes.Stelem_I8;

                case TypeCode.Single:
                    return OpCodes.Stelem_R4;

                case TypeCode.Double:
                    return OpCodes.Stelem_R8;

                case TypeCode.String:
                    return OpCodes.Stelem_Ref;
            }
            return OpCodes.Nop;
        }

        /// <summary>
        /// GetVariableType
        /// </summary>
        /// <param name="var"></param>
        /// <returns></returns>
        public Type GetVariableType(object var)
        {
            if (var is ArgBuilder)
            {
                return ((ArgBuilder)var).ArgType;
            }
            if (var is LocalBuilder)
            {
                return ((LocalBuilder)var).LocalType;
            }
            return var.GetType();
        }

        /// <summary>
        /// If
        /// </summary>
        public void If()
        {
            this.InternalIf(false);
        }

        /// <summary>
        /// If
        /// </summary>
        /// <param name="cmpOp"></param>
        public void If(Cmp cmpOp)
        {
            IfState state = new IfState();
            state.EndIf = this.DefineLabel();
            state.ElseBegin = this.DefineLabel();
            this.ilGen.Emit(this.GetBranchCode(cmpOp), state.ElseBegin);
            this.blockStack.Push(state);
        }

        /// <summary>
        /// If
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="cmpOp"></param>
        /// <param name="value2"></param>
        public void If(object value1, Cmp cmpOp, object value2)
        {
            this.Load(value1);
            this.Load(value2);
            this.If(cmpOp);
        }

        /// <summary>
        /// IfFalseBreak
        /// </summary>
        /// <param name="forState"></param>
        public void IfFalseBreak(object forState)
        {
            this.InternalBreakFor(forState, OpCodes.Brfalse);
        }

        /// <summary>
        /// IfNot
        /// </summary>
        public void IfNot()
        {
            this.InternalIf(true);
        }

        /// <summary>
        /// IfNotDefaultValue
        /// </summary>
        /// <param name="value"></param>
        public void IfNotDefaultValue(object value)
        {
            Type variableType = this.GetVariableType(value);
            TypeCode typeCode = Type.GetTypeCode(variableType);
            if (((typeCode == TypeCode.Object) && variableType.IsValueType) || ((typeCode == TypeCode.DateTime) || (typeCode == TypeCode.Decimal)))
            {
                this.LoadDefaultValue(variableType);
                this.ConvertValue(variableType, typeof(object));
                this.Load(value);
                this.ConvertValue(variableType, typeof(object));
                this.Call(ObjectEquals);
                this.IfNot();
            }
            else
            {
                this.LoadDefaultValue(variableType);
                this.Load(value);
                this.If(Cmp.NotEqualTo);
            }
        }

        /// <summary>
        /// IfTrueBreak
        /// </summary>
        /// <param name="forState"></param>
        public void IfTrueBreak(object forState)
        {
            this.InternalBreakFor(forState, OpCodes.Brtrue);
        }

        /// <summary>
        /// IgnoreReturnValue
        /// </summary>
        public void IgnoreReturnValue()
        {
            this.Pop();
        }

        /// <summary>
        /// Inc
        /// </summary>
        /// <param name="var"></param>
        public void Inc(object var)
        {
            this.Load(var);
            this.Load(1);
            this.Add();
            this.Store(var);
        }

        /// <summary>
        /// InitObj
        /// </summary>
        /// <param name="valueType"></param>
        public void InitObj(Type valueType)
        {
            Check.Require(valueType, "valueType");

            this.ilGen.Emit(OpCodes.Initobj, valueType);
        }

        /// <summary>
        /// InternalBreakFor
        /// </summary>
        /// <param name="userForState"></param>
        /// <param name="branchInstruction"></param>
        public void InternalBreakFor(object userForState, OpCode branchInstruction)
        {
            foreach (object obj2 in this.blockStack)
            {
                ForState state = obj2 as ForState;
                if ((state != null) && (state == userForState))
                {
                    if (!state.RequiresEndLabel)
                    {
                        state.EndLabel = this.DefineLabel();
                        state.RequiresEndLabel = true;
                    }
                    this.ilGen.Emit(branchInstruction, state.EndLabel);
                    break;
                }
            }
        }

        /// <summary>
        /// InternalConvert
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="isAddress"></param>
        private void InternalConvert(Type source, Type target, bool isAddress)
        {
            if (target != source)
            {
                if (target.IsValueType)
                {
                    if (source.IsValueType)
                    {
                        OpCode opcode = this.GetConvOpCode(Type.GetTypeCode(target));
                        if (opcode.Equals(OpCodes.Nop))
                        {
                            throw new Exception("NoConversionPossible");
                        }
                        this.ilGen.Emit(opcode);
                    }
                    else
                    {
                        if (!source.IsAssignableFrom(target))
                        {
                            throw new Exception("IsNotAssignableFrom");
                        }
                        this.Unbox(target);
                        if (!isAddress)
                        {
                            this.Ldobj(target);
                        }
                    }
                }
                else if (target.IsAssignableFrom(source))
                {
                    if (source.IsValueType)
                    {
                        if (isAddress)
                        {
                            this.Ldobj(source);
                        }
                        this.Box(source);
                    }
                }
                else if (source.IsAssignableFrom(target))
                {
                    this.Castclass(target);
                }
                else
                {
                    if (!target.IsInterface && !source.IsInterface)
                    {
                        throw new Exception("IsNotAssignableFrom");
                    }
                    this.Castclass(target);
                }
            }
        }

        private void InternalIf(bool negate)
        {
            IfState state = new IfState();
            state.EndIf = this.DefineLabel();
            state.ElseBegin = this.DefineLabel();
            if (negate)
            {
                this.Brtrue(state.ElseBegin);
            }
            else
            {
                this.Brfalse(state.ElseBegin);
            }
            this.blockStack.Push(state);
        }

        private static bool IsStruct(Type objType)
        {
            if (objType.IsValueType)
            {
                return !objType.IsPrimitive;
            }
            return false;
        }

        /// <summary>
        /// Ldarg 加载第solt参数到堆栈
        /// 注意：实例方法都有一个隐含参数，也就是第0个参数是表示当前对象引用 也就是 this指针
        /// </summary>
        /// <param name="slot">The slot.</param>
        public void Ldarg(int slot)
        {
            switch (slot)
            {
                case 0:
                    this.ilGen.Emit(OpCodes.Ldarg_0);
                    return;

                case 1:
                    this.ilGen.Emit(OpCodes.Ldarg_1);
                    return;

                case 2:
                    this.ilGen.Emit(OpCodes.Ldarg_2);
                    return;

                case 3:
                    this.ilGen.Emit(OpCodes.Ldarg_3);
                    return;
            }
            if (slot <= 0xff)
            {
                //将参数（由指定的短格式索引引用）加载到计算堆栈上。
                this.ilGen.Emit(OpCodes.Ldarg_S, slot);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Ldarg, slot);
            }
        }

        /// <summary>
        /// Ldarg 调用Ldarg(int)的重载方式
        /// </summary>
        /// <param name="arg"></param>
        public void Ldarg(ArgBuilder arg)
        {
            Check.Require(arg, "arg");

            this.Ldarg(arg.Index);
        }

        /// <summary>
        /// Ldarga 将参数的地址加载到计算堆栈
        /// 自动判断是使用Ldarga_S,还是Ldarga
        /// Ldarga_S 指令是用于slot值在 0 到 255的参数编号，效率更高的编码
        /// </summary>
        /// <param name="slot"></param>
        public void Ldarga(int slot)
        {
            if (slot <= 0xff)
            {
                this.ilGen.Emit(OpCodes.Ldarga_S, slot);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Ldarga, slot);
            }
        }

        /// <summary>
        /// Ldarga 调用 Ldarga(int) 重载
        /// </summary>
        /// <param name="argBuilder"></param>
        public void Ldarga(ArgBuilder argBuilder)
        {
            Check.Require(argBuilder, "argBuilder");

            this.Ldarga(argBuilder.Index);
        }

        /// <summary>
        /// LdargAddress 加载参数地址。IL没有这个指令，它根据参数的类型分别调用不同的IL指令：
        /// 当参数为值类型时，通过调用Ldarga指令，加载参数地址
        /// 当参不为值类型时，直接调用Ldarg加载参数
        /// </summary>
        /// <param name="argBuilder"></param>
        public void LdargAddress(ArgBuilder argBuilder)
        {
            Check.Require(argBuilder, "argBuilder");

            if (argBuilder.ArgType.IsValueType)
            {
                this.Ldarga(argBuilder);
            }
            else
            {
                this.Ldarg(argBuilder);
            }
        }

        /// <summary>
        /// Ldc 通过Ldc指令加载布尔值到计算堆栈 true 为 1,false 为0        
        /// </summary>
        /// <param name="boolVar"></param>
        public void Ldc(bool boolVar)
        {
            if (boolVar)
            {
                this.ilGen.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Ldc_I4_0);
            }
        }

        /// <summary>
        /// Ldc 加载双精度浮点数到计算堆栈 对应指令：Ldc_R8
        /// </summary>
        /// <param name="d"></param>
        public void Ldc(double d)
        {
            this.ilGen.Emit(OpCodes.Ldc_R8, d);
        }

        /// <summary>
        /// Ldc 加载整型数值到计算堆栈
        /// <list type="">
        /// ldc.i4.m1 (ldc.i4.M1)    -1 
        /// ldc.i4.0                0
        /// ...
        /// ldc.i4.8                8
        /// Ldc_I4                  普通整数
        /// </list>
        /// </summary>
        /// <param name="intVar"></param>
        public void Ldc(int intVar)
        {
            switch (intVar)
            {
                case -1:
                    this.ilGen.Emit(OpCodes.Ldc_I4_M1);
                    return;

                case 0:
                    this.ilGen.Emit(OpCodes.Ldc_I4_0);
                    return;

                case 1:
                    this.ilGen.Emit(OpCodes.Ldc_I4_1);
                    return;

                case 2:
                    this.ilGen.Emit(OpCodes.Ldc_I4_2);
                    return;

                case 3:
                    this.ilGen.Emit(OpCodes.Ldc_I4_3);
                    return;

                case 4:
                    this.ilGen.Emit(OpCodes.Ldc_I4_4);
                    return;

                case 5:
                    this.ilGen.Emit(OpCodes.Ldc_I4_5);
                    return;

                case 6:
                    this.ilGen.Emit(OpCodes.Ldc_I4_6);
                    return;

                case 7:
                    this.ilGen.Emit(OpCodes.Ldc_I4_7);
                    return;

                case 8:
                    this.ilGen.Emit(OpCodes.Ldc_I4_8);
                    return;
            }
            this.ilGen.Emit(OpCodes.Ldc_I4, intVar);
        }

        /// <summary>
        /// Ldc 加载长整数到计算堆栈   对应指令 ldc.i8 
        /// </summary>
        /// <param name="l">The l.</param>
        public void Ldc(long l)
        {
            this.ilGen.Emit(OpCodes.Ldc_I8, l);
        }

        /// <summary>
        /// Ldc 传入一个未知类型对象，根据其类型，自动调用适合的指令来将它加载到计算堆栈
        /// </summary>
        /// <param name="o"></param>
        public void Ldc(object o)
        {
            Type enumType = o.GetType();
            if (o is Type)
            {
                this.Ldtoken((Type)o);
                this.Call(GetTypeFromHandle);
            }
            else if (enumType.IsEnum)
            {
                this.Ldc(((IConvertible)o).ToType(Enum.GetUnderlyingType(enumType), null));
            }
            else
            {
                switch (Type.GetTypeCode(enumType))
                {
                    case TypeCode.Boolean:
                        this.Ldc((bool)o);
                        return;

                    case TypeCode.Char:
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.GetString("CharIsInvalidPrimitive")));

                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                        this.Ldc(((IConvertible)o).ToInt32(CultureInfo.InvariantCulture));
                        return;

                    case TypeCode.Int32:
                        this.Ldc((int)o);
                        return;

                    case TypeCode.UInt32:
                        this.Ldc((int)((uint)o));
                        return;

                    case TypeCode.Int64:
                        this.Ldc((long)o);
                        return;

                    case TypeCode.UInt64:
                        this.Ldc((long)((ulong)o));
                        return;

                    case TypeCode.Single:
                        this.Ldc((float)o);
                        return;

                    case TypeCode.Double:
                        this.Ldc((double)o);
                        return;

                    case TypeCode.String:
                        this.Ldstr((string)o);
                        return;
                }



                throw new Exception("UnknownConstantType");
            }
        }

        /// <summary>
        /// Ldc 加载单精度浮点数到计算堆栈
        /// </summary>
        /// <param name="f"></param>
        public void Ldc(float f)
        {
            this.ilGen.Emit(OpCodes.Ldc_R4, f);
        }

        /// <summary>
        /// Ldelem 根据传入的数组类型，自动选择不同的指令，将位于数组指定索引处的元素加到计算堆栈 
        /// 这些指令包括：
        /// ldelem.i1       加载int8
        /// ldelem.u1       unsigned int8.
        /// ldelem.i2       int16
        /// ldelem.u2       unsigned int16
        /// ldelem.i4       int32
        /// ldelem.i8       int64
        /// ldelem.i        native int
        /// ldelem.r4       float32
        /// ldelem.r8       float64
        /// ldelem.ref      reference type
        /// </summary>
        /// <param name="arrayElementType"></param>
        public void Ldelem(Type arrayElementType)
        {
            Check.Require(arrayElementType, "arrayElementType");

            if (arrayElementType.IsEnum)
            {
                this.Ldelem(Enum.GetUnderlyingType(arrayElementType));
            }
            else
            {
                OpCode opcode = this.GetLdelemOpCode(Type.GetTypeCode(arrayElementType));
                if (opcode.Equals(OpCodes.Nop))
                {
                    throw new Exception("ArrayTypeIsNotSupported");
                }
                this.ilGen.Emit(opcode);
            }
        }

        /// <summary>
        /// Ldelema 将位于指定数组索引的数组元素的地址作为 & 类型（托管指针）加载到计算堆栈的顶部。
        /// </summary>
        /// <param name="arrayElementType"></param>
        public void Ldelema(Type arrayElementType)
        {
            Check.Require(arrayElementType, "arrayElementType");

            OpCode opcode = OpCodes.Ldelema;
            this.ilGen.Emit(opcode, arrayElementType);
        }

        /// <summary>
        /// Ldlen 将从零开始的、一维数组的元素的数目推送到计算堆栈上。        
        /// </summary>
        public void Ldlen()
        {
            this.ilGen.Emit(OpCodes.Ldlen);
            this.ilGen.Emit(OpCodes.Conv_I4);
        }

        /// <summary>
        /// Ldloc 将指定索引处的局部变量加载到计算堆栈上。
        /// </summary>
        /// <param name="slot"></param>
        public void Ldloc(int slot)
        {
            switch (slot)
            {
                case 0:
                    this.ilGen.Emit(OpCodes.Ldloc_0);
                    return;

                case 1:
                    this.ilGen.Emit(OpCodes.Ldloc_1);
                    return;

                case 2:
                    this.ilGen.Emit(OpCodes.Ldloc_2);
                    return;

                case 3:
                    this.ilGen.Emit(OpCodes.Ldloc_3);
                    return;
            }
            if (slot <= 0xff)
            {
                this.ilGen.Emit(OpCodes.Ldloc_S, slot);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Ldloc, slot);
            }
        }

        /// <summary>
        /// Ldloc 将指定索引处的局部变量加载到计算堆栈上
        /// </summary>
        /// <param name="localBuilder"></param>
        public void Ldloc(LocalBuilder localBuilder)
        {
            this.ilGen.Emit(OpCodes.Ldloc, localBuilder);
        }

        /// <summary>
        /// Ldloca 将位于特定索引处的局部变量的地址加载到计算堆栈上。
        /// </summary>
        /// <param name="slot"></param>
        public void Ldloca(int slot)
        {
            if (slot <= 0xff)
            {
                this.ilGen.Emit(OpCodes.Ldloca_S, slot);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Ldloca, slot);
            }
        }

        /// <summary>
        /// Ldloca
        /// </summary>
        /// <param name="localBuilder"></param>
        public void Ldloca(LocalBuilder localBuilder)
        {
            Check.Require(localBuilder, "localBuilder");

            this.ilGen.Emit(OpCodes.Ldloca, localBuilder);
        }

        /// <summary>
        /// LdlocAddress 不是IL指令。
        /// 分别调用Ldloca和Ldloc将值/引用类型变量地址加载到计算堆栈
        /// 当是为值类型时调用 Ldloca
        /// 否则调用 Ldloc
        /// </summary>
        /// <param name="localBuilder"></param>
        public void LdlocAddress(LocalBuilder localBuilder)
        {
            Check.Require(localBuilder, "localBuilder");

            if (localBuilder.LocalType.IsValueType)
            {
                this.Ldloca(localBuilder);
            }
            else
            {
                this.Ldloc(localBuilder);
            }
        }

        /// <summary>
        /// Ldobj 将地址指向的值类型(类型为type)对象复制到计算堆栈的顶部        
        /// </summary>
        /// <param name="type"></param>
        public void Ldobj(Type type)
        {
            Check.Require(type, "type");

            OpCode opcode = this.GetLdindOpCode(Type.GetTypeCode(type));
            if (!opcode.Equals(OpCodes.Nop))
            {
                this.ilGen.Emit(opcode);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Ldobj, type);
            }
        }

        /// <summary>
        /// Ldstr
        /// 将一个字符串加载到 Stack
        /// </summary>
        /// <param name="strVar"></param>
        public void Ldstr(string strVar)
        {
            this.ilGen.Emit(OpCodes.Ldstr, strVar);
        }

        /// <summary>
        /// Ldtoken 将元数据标记(数据类型)转换为其运行时表示形式，并将其推送到计算堆栈上。
        /// </summary>
        /// <param name="t"></param>
        public void Ldtoken(Type t)
        {
            Check.Require(t, "t");

            this.ilGen.Emit(OpCodes.Ldtoken, t);
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="obj"></param>
        public void Load(object obj)
        {
            if (obj == null)
            {
                this.ilGen.Emit(OpCodes.Ldnull);
            }
            else if (obj is ArgBuilder)
            {
                this.Ldarg((ArgBuilder)obj);
            }
            else if (obj is LocalBuilder)
            {
                this.Ldloc((LocalBuilder)obj);
            }
            else
            {
                this.Ldc(obj);
            }
        }

        /// <summary>
        /// LoadAddress
        /// </summary>
        /// <param name="obj"></param>
        public void LoadAddress(object obj)
        {
            if (obj is ArgBuilder)
            {
                this.LdargAddress((ArgBuilder)obj);
            }
            else if (obj is LocalBuilder)
            {
                this.LdlocAddress((LocalBuilder)obj);
            }
            else
            {
                this.Load(obj);
            }
        }

        /// <summary>
        /// LoadArrayElement
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="arrayIndex"></param>
        public void LoadArrayElement(object obj, object arrayIndex)
        {
            Type objType = this.GetVariableType(obj).GetElementType();
            this.Load(obj);
            this.Load(arrayIndex);
            if (IsStruct(objType))
            {
                this.Ldelema(objType);
                this.Ldobj(objType);
            }
            else
            {
                this.Ldelem(objType);
            }
        }

        /// <summary>
        /// LoadDefaultValue
        /// </summary>
        /// <param name="type"></param>
        public void LoadDefaultValue(Type type)
        {
            Check.Require(type, "type");

            if (!type.IsValueType)
            {
                this.Load(null);
            }
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        this.Ldc(false);
                        return;

                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        this.Ldc(0);
                        return;

                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        this.Ldc((long)0);
                        return;

                    case TypeCode.Single:
                        this.Ldc((float)0f);
                        return;

                    case TypeCode.Double:
                        this.Ldc((double)0);
                        return;
                        

                }
                LocalBuilder builder = this.DeclareLocal(type, "zero");
                this.LoadAddress(builder);
                this.InitObj(type);
                this.Load(builder);
            }
        }

        /// <summary>
        /// LoadMember
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public Type LoadMember(MemberInfo memberInfo)
        {
            Check.Require(memberInfo, "memberInfo");

            Type stackTopType = null;
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)memberInfo;
                stackTopType = field.FieldType;
                if (field.IsStatic)
                {
                    this.ilGen.Emit(OpCodes.Ldsfld, field);
                }
                else
                {
                    this.ilGen.Emit(OpCodes.Ldfld, field);
                }
            }
            else if (memberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo info2 = memberInfo as PropertyInfo;
                stackTopType = info2.PropertyType;
                if (info2 != null)
                {
                    MethodInfo methodInfo = info2.GetGetMethod(true);
                    if (methodInfo == null)
                    {
                        throw new Exception("NoGetMethodForProperty");
                    }
                    this.Call(methodInfo);
                }
            }
            else
            {
                if (memberInfo.MemberType != MemberTypes.Method)
                {
                    throw new Exception("CannotLoadMemberType");
                }
                MethodInfo info4 = (MethodInfo)memberInfo;
                stackTopType = info4.ReturnType;
                this.Call(info4);
            }
            return stackTopType;
        }

        /// <summary>
        /// LoadParam
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="oneBasedArgIndex"></param>
        /// <param name="methodInfo"></param>
        private void LoadParam(object arg, int oneBasedArgIndex, MethodBase methodInfo)
        {
            this.Load(arg);
            if (arg != null)
            {
                this.ConvertValue(this.GetVariableType(arg), methodInfo.GetParameters()[oneBasedArgIndex - 1].ParameterType);
            }
        }

        /// <summary>
        /// LoadThis
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="methodInfo"></param>
        private void LoadThis(object thisObj, MethodInfo methodInfo)
        {
            if ((thisObj != null) && !methodInfo.IsStatic)
            {
                this.LoadAddress(thisObj);
                this.ConvertAddress(this.GetVariableType(thisObj), methodInfo.DeclaringType);
            }
        }

        /// <summary>
        /// MarkLabel
        /// </summary>
        /// <param name="label"></param>
        public void MarkLabel(Label label)
        {
            this.ilGen.MarkLabel(label);
        }

        /// <summary>
        /// New
        /// </summary>
        /// <param name="constructorInfo"></param>
        public void New(ConstructorInfo constructorInfo)
        {
            Check.Require(constructorInfo, "constructorInfo");

            this.ilGen.Emit(OpCodes.Newobj, constructorInfo);
        }

        /// <summary>
        /// New
        /// </summary>
        /// <param name="constructorInfo"></param>
        /// <param name="param1"></param>
        public void New(ConstructorInfo constructorInfo, object param1)
        {
            Check.Require(constructorInfo, "constructorInfo");

            this.LoadParam(param1, 1, constructorInfo);
            this.New(constructorInfo);
        }

        /// <summary>
        /// NewArray
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="len"></param>
        public void NewArray(Type elementType, object len)
        {
            Check.Require(elementType, "elementType");

            this.Load(len);
            this.ilGen.Emit(OpCodes.Newarr, elementType);
        }

        /// <summary>
        /// Not
        /// </summary>
        public void Not()
        {
            this.ilGen.Emit(OpCodes.Not);
        }

        /// <summary>
        /// Or
        /// </summary>
        public void Or()
        {
            this.ilGen.Emit(OpCodes.Or);
        }

        /// <summary>
        /// Pop
        /// </summary>
        public void Pop()
        {
            this.ilGen.Emit(OpCodes.Pop);
        }

        private IfState PopIfState()
        {
            object expected = this.blockStack.Pop();
            IfState state = expected as IfState;
            if (state == null)
            {
                this.ThrowMismatchException(expected);
            }
            return state;
        }

        /// <summary>
        /// Ret 从当前方法返回，并将返回值（如果存在）从调用方的计算堆栈推送到被调用方的计算堆栈上。
        /// </summary>
        public void Ret()
        {
            this.ilGen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Set
        /// </summary>
        /// <param name="local"></param>
        /// <param name="value"></param>
        public void Set(LocalBuilder local, object value)
        {
            Check.Require(local, "local");

            this.Load(value);
            this.Store(local);
        }

        /// <summary>
        /// Starg
        /// </summary>
        /// <param name="slot"></param>
        public void Starg(int slot)
        {
            if (slot <= 0xff)
            {
                this.ilGen.Emit(OpCodes.Starg_S, slot);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Starg, slot);
            }
        }

        /// <summary>
        /// Starg
        /// </summary>
        /// <param name="arg"></param>
        public void Starg(ArgBuilder arg)
        {
            Check.Require(arg, "arg");

            this.Starg(arg.Index);
        }

        /// <summary>
        /// Stelem
        /// </summary>
        /// <param name="arrayElementType"></param>
        public void Stelem(Type arrayElementType)
        {
            Check.Require(arrayElementType, "arrayElementType");

            if (arrayElementType.IsEnum)
            {
                this.Stelem(Enum.GetUnderlyingType(arrayElementType));
            }
            else
            {
                OpCode opcode = this.GetStelemOpCode(Type.GetTypeCode(arrayElementType));
                if (opcode.Equals(OpCodes.Nop))
                {
                    throw new Exception("ArrayTypeIsNotSupported");
                }
                this.ilGen.Emit(opcode);
            }
        }

        /// <summary>
        /// Stloc
        /// 从计算堆栈的顶部弹出当前值并将其存储到指定索引处的局部变量列表中。
        /// </summary>
        /// <param name="slot"></param>
        public void Stloc(int slot)
        {
            switch (slot)
            {
                case 0:
                    this.ilGen.Emit(OpCodes.Stloc_0);
                    return;

                case 1:
                    this.ilGen.Emit(OpCodes.Stloc_1);
                    return;

                case 2:
                    this.ilGen.Emit(OpCodes.Stloc_2);
                    return;

                case 3:
                    this.ilGen.Emit(OpCodes.Stloc_3);
                    return;
            }
            if (slot <= 0xff)
            {
                this.ilGen.Emit(OpCodes.Stloc_S, slot);
            }
            else
            {
                this.ilGen.Emit(OpCodes.Stloc, slot);
            }
        }

        /// <summary>
        /// Stloc
        /// </summary>
        /// <param name="local"></param>
        public void Stloc(LocalBuilder local)
        {
            Check.Require(local, "local");

            this.ilGen.Emit(OpCodes.Stloc, local);
        }

        /// <summary>
        /// Stobj
        /// </summary>
        /// <param name="type"></param>
        public void Stobj(Type type)
        {
            Check.Require(type, "type");

            this.ilGen.Emit(OpCodes.Stobj, type);
        }

        /// <summary>
        /// Store
        /// </summary>
        /// <param name="var"></param>
        public void Store(object var)
        {
            if (var is ArgBuilder)
            {
                this.Starg((ArgBuilder)var);
            }
            else
            {
                if (!(var is LocalBuilder))
                {
                    throw new Exception("CanOnlyStoreIntoArgOrLocGot0");
                }
                this.Stloc((LocalBuilder)var);
            }
        }

        /// <summary>
        /// StoreArrayElement
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="value"></param>
        public void StoreArrayElement(object obj, object arrayIndex, object value)
        {
            Type variableType = this.GetVariableType(obj);
            Type objType = (variableType == typeof(Array)) ? typeof(object) : variableType.GetElementType();
            this.Load(obj);
            this.Load(arrayIndex);
            if (IsStruct(objType))
            {
                this.Ldelema(objType);
            }
            this.Load(value);
            this.ConvertValue(this.GetVariableType(value), objType);
            if (IsStruct(objType))
            {
                this.Stobj(objType);
            }
            else
            {
                this.Stelem(objType);
            }
        }

        /// <summary>
        /// StoreMember 用新值替换在对象引用或指针的字段中存储的值。
        /// </summary>
        /// <param name="memberInfo"></param>
        public void StoreMember(MemberInfo memberInfo)
        {
            Check.Require(memberInfo, "memberInfo");

            if (memberInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)memberInfo;
                if (field.IsStatic)
                {
                    this.ilGen.Emit(OpCodes.Stsfld, field);
                }
                else
                {
                    this.ilGen.Emit(OpCodes.Stfld, field);
                }
            }
            else if (memberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo info2 = memberInfo as PropertyInfo;
                if (info2 != null)
                {
                    MethodInfo methodInfo = info2.GetSetMethod(true);
                    if (methodInfo == null)
                    {
                        throw new Exception("NoSetMethodForProperty");
                    }
                    this.Call(methodInfo);
                }
            }
            else
            {
                if (memberInfo.MemberType != MemberTypes.Method)
                {
                    throw new Exception("CannotLoadMemberType");
                }
                this.Call((MethodInfo)memberInfo);
            }
        }

        /// <summary>
        /// Subtract
        /// </summary>
        public void Subtract()
        {
            this.ilGen.Emit(OpCodes.Sub);
        }

        /// <summary>
        /// Switch
        /// </summary>
        /// <param name="labelCount"></param>
        /// <returns></returns>
        public Label[] Switch(int labelCount)
        {
            SwitchState state = new SwitchState(this.DefineLabel(), this.DefineLabel());
            Label[] labels = new Label[labelCount];
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = this.DefineLabel();
            }
            this.ilGen.Emit(OpCodes.Switch, labels);
            this.Br(state.DefaultLabel);
            this.blockStack.Push(state);
            return labels;
        }

        /// <summary>
        /// Throw
        /// </summary>
        public void Throw()
        {
            this.ilGen.Emit(OpCodes.Throw);
        }

        private void ThrowMismatchException(object expected)
        {
            throw new Exception("ExpectingEnd");
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <param name="type"></param>
        public void ToString(Type type)
        {
            Check.Require(type, "type");

            if (type.IsValueType)
            {
                this.Box(type);
                this.Call(ObjectToString);
            }
            else
            {
                this.Dup();
                this.Load(null);
                this.If(Cmp.EqualTo);
                this.Pop();
                this.Load("<null>");
                this.Else();
                if (type.IsArray)
                {
                    LocalBuilder var = this.DeclareLocal(type, "arrayVar");
                    this.Store(var);
                    this.Load("{ ");
                    LocalBuilder builder2 = this.DeclareLocal(typeof(string), "arrayValueString");
                    this.Store(builder2);
                    LocalBuilder local = this.DeclareLocal(typeof(int), "i");
                    this.For(local, 0, var);
                    this.Load(builder2);
                    this.LoadArrayElement(var, local);
                    this.ToString(var.LocalType.GetElementType());
                    this.Load(", ");
                    this.Concat3();
                    this.Store(builder2);
                    this.EndFor();
                    this.Load(builder2);
                    this.Load("}");
                    this.Concat2();
                }
                else
                {
                    this.Call(ObjectToString);
                }
                this.EndIf();
            }
        }

        /// <summary>
        /// Unbox
        /// </summary>
        /// <param name="type"></param>
        public void Unbox(Type type)
        {
            Check.Require(type, "type");
            Check.Require(type.IsValueType, "type MUST be ValueType");

            this.ilGen.Emit(OpCodes.Unbox, type);
        }

        /// <summary>
        /// UnboxAny
        /// </summary>
        /// <param name="type"></param>
        public void UnboxAny(Type type)
        {
            Check.Require(type, "type");
            Check.Require(type.IsValueType, "type MUST be ValueType");

            this.ilGen.Emit(OpCodes.Unbox_Any, type);
        }

        /// <summary>
        /// VerifyParameterCount
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="expectedCount"></param>
        public void VerifyParameterCount(MethodInfo methodInfo, int expectedCount)
        {
            Check.Require(methodInfo, "methodInfo");

            if (methodInfo.GetParameters().Length != expectedCount)
            {
                throw new Exception("ParameterCountMismatch");
            }
        }

        /// <summary>
        /// CurrentMethod
        /// </summary>
        public MethodInfo CurrentMethod
        {
            get
            {
                return (this.dynamicMethod as MethodInfo) ?? (this.methodOrConstructorBuilder as MethodInfo);
            }
        }

        /// <summary>
        /// InternalILGenerator
        /// </summary>
        public ILGenerator InternalILGenerator
        {
            get
            {
                return this.ilGen;
            }
        }

        /// <summary>
        /// SerializationModule
        /// </summary>
        public Module SerializationModule
        {
            get
            {
                return this.methodOrConstructorBuilder == null ? this.serializationModule : this.methodOrConstructorBuilder.Module;
            }
        }

        private static MethodInfo GetTypeFromHandle
        {
            get
            {
                if (getTypeFromHandle == null)
                {
                    getTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
                }
                return getTypeFromHandle;
            }
        }

        private Hashtable LocalNames
        {
            get
            {
                if (this.localNames == null)
                {
                    this.localNames = new Hashtable();
                }
                return this.localNames;
            }
        }

        private static MethodInfo ObjectEquals
        {
            get
            {
                if (objectEquals == null)
                {
                    objectEquals = typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static);
                }
                return objectEquals;
            }
        }

        private static MethodInfo ObjectToString
        {
            get
            {
                if (objectToString == null)
                {
                    objectToString = typeof(object).GetMethod("ToString", new Type[0]);
                }
                return objectToString;
            }
        }

        private static MethodInfo StringConcat2
        {
            get
            {
                if (stringConcat2 == null)
                {
                    stringConcat2 = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
                }
                return stringConcat2;
            }
        }

        private static MethodInfo StringConcat3
        {
            get
            {
                if (stringConcat3 == null)
                {
                    stringConcat3 = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string), typeof(string) });
                }
                return stringConcat3;
            }
        }

        private static MethodInfo StringFormat
        {
            get
            {
                if (stringFormat == null)
                {
                    stringFormat = typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object[]) });
                }
                return stringFormat;
            }
        }
    }
}
