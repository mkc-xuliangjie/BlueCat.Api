using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
namespace BlueCat.ORM.Mapping
{
    public class ObjectGetterGenerator : GeneratorDriver, IMemberGetterGenerator
    {
        /// <summary>
        /// 	
        /// </summary>
        public ObjectGetterGenerator() { }

        /// <summary>
        /// 	inherite base 
        /// </summary>
        /// <param name="memberGetterGenerator">The member getter generator.</param>
        /// <param name="memberSetterGenerator">The member setter generator.</param>
        public ObjectGetterGenerator(IMemberGetterGenerator memberGetterGenerator, IMemberSetterGenerator memberSetterGenerator)
            : base(memberGetterGenerator, memberSetterGenerator)
        {
        }

        #region IMemberGetterGenerator Members

        public void GetMemberValue(CodeGenerator generator, ArgBuilder targetObject, string memberName, ref LocalBuilder memberValue)
        {
            Type inputType = targetObject.ArgType;
            MemberInfo memberInfo = ConvertorGeneratorHelper.GetMemberInfo(inputType, memberName);
            if (memberInfo == null)
            {
                return;
            }
            Type srcMemberType = ConvertorGeneratorHelper.GetMemberType(memberInfo);
            Type destMemberType = memberValue.LocalType;
            if (memberValue.LocalType.IsAssignableFrom(srcMemberType))
            {
                generator.Ldarg(targetObject);
                generator.LoadMember(memberInfo);
                generator.Stloc(memberValue);
                return;
            }

            Type converTargetType = ConvertorGeneratorHelper.GetOriginalType(destMemberType);

            Type convertSourceType = ConvertorGeneratorHelper.GetOriginalType(srcMemberType);

            //源字段类型是否实现了IConvertible
            if (typeof(IConvertible).IsAssignableFrom(convertSourceType))
            {
                generator.Ldarg(targetObject);
                generator.LoadMember(memberInfo);
                if (convertSourceType.IsValueType)
                {
                    //一定要是原类型
                    generator.Box(srcMemberType);
                }
                generator.Load(converTargetType);
                generator.Call(typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) }));
                if (destMemberType.IsValueType)
                {
                    generator.UnboxAny(destMemberType);
                }
                generator.Stloc(memberValue);
            }
        }

        #endregion

        public override ConvertHandler GetConvertHandler(Type inputType, Type outputType, object inputObject, object outputObject, Dictionary<string, string> mappingNames, List<string> ignoreList, Dictionary<int, string> mappingOrders, bool mappingSpecifiedOnly)
        {
            //保存目标字段名，目标MemberInfo
            Dictionary<string, string> distToSrc = new Dictionary<string, string>();
            Dictionary<string, MemberInfo> members = new Dictionary<string, MemberInfo>();
            //先查找显式映射
            foreach (string sourceName in mappingNames.Keys)
            {
                MemberInfo sourceMember = ConvertorGeneratorHelper.GetMemberInfo(inputType, sourceName);
                Check.Require(sourceMember != null, string.Format("member named {0} could not be found in {1}", sourceName, outputType.FullName));
                distToSrc.Add(mappingNames[sourceName], sourceName);
                members.Add(mappingNames[sourceName], sourceMember);
            }
            //如果不是只显式映射
            if (!mappingSpecifiedOnly)
            {
                Dictionary<string, MemberInfo> sourceMembers = ConvertorGeneratorHelper.GetMembers(inputType);
                foreach (string sourceName in sourceMembers.Keys)
                {
                    if (!ignoreList.Contains(sourceName) && !distToSrc.ContainsKey(sourceName))
                    {
                        distToSrc.Add(sourceName, sourceName);
                        members.Add(sourceName, sourceMembers[sourceName]);
                    }
                }
            }

            CodeGenerator gen = new CodeGenerator();
            gen.BeginMethod("m" + Guid.NewGuid().ToString("N"), typeof(ConvertHandler));

            ArgBuilder inputObjectArg = new ArgBuilder(0, inputType);
            ArgBuilder outputObjectArg = new ArgBuilder(1, outputType);

            int currentCount = 0;
            int memberCount = members.Count;
            string[] keys = new string[memberCount];
            distToSrc.Keys.CopyTo(keys, 0);
            List<string> keyList = new List<string>(keys);
            MemberSetterGenerator.BeginSetMembers(gen, outputObjectArg);
            //按排序顺序
            while (currentCount < memberCount)
            {
                currentCount++;
                string targetName = ConvertorGeneratorHelper.GetCurrentKey(mappingOrders, currentCount, keyList);
                if (string.IsNullOrEmpty(targetName))
                {
                    continue;
                }
                string sourceName = distToSrc[targetName];
                if (MemberGetterGenerator.ContainsMember(sourceName, inputType, inputObject) && MemberSetterGenerator.ContainsMember(targetName, outputType, outputObject))
                {
                    Type targetType = ConvertorGeneratorHelper.GetMemberType(members[targetName]);

                    LocalBuilder memberValue = gen.DeclareLocal(targetType, "memberValue");

                    MemberGetterGenerator.GetMemberValue(gen, inputObjectArg, sourceName, ref memberValue);

                    MemberSetterGenerator.SetMemberValue(gen, outputObjectArg, targetName, memberValue);
                }
            }
            gen.Ldarg(outputObjectArg);
            MemberSetterGenerator.EndSetMembers(gen, outputObjectArg);
            return (ConvertHandler)gen.EndMethod();
        }

        #region IMemberGetterGenerator Members

        public bool ContainsMember(string name, Type type, object inputObject)
        {
            return ConvertorGeneratorHelper.GetMemberInfo(type, name) != null;
        }

        #endregion
    }
}
