using System;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace BlueCat.ORM.Mapping
{
    /// <summary>
    /// object setter generator
    /// </summary>
    public class ObjectSetterGenerator : GeneratorDriver, IMemberSetterGenerator
    {
        /// <summary>
        /// 	
        /// </summary>
        public ObjectSetterGenerator() { }

        /// <summary>
        /// 	inherite base 
        /// </summary>
        /// <param name="memberGetterGenerator">The member getter generator.</param>
        /// <param name="memberSetterGenerator">The member setter generator.</param>
        public ObjectSetterGenerator(IMemberGetterGenerator memberGetterGenerator, IMemberSetterGenerator memberSetterGenerator)
            : base(memberGetterGenerator, memberSetterGenerator)
        {
        }

        #region IGeneratorDriver Members

        /// <summary>
        /// 	get convert handler
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="mappingNames">The mapping names.</param>
        /// <param name="ignoreList">The ignore list.</param>
        /// <param name="mappingOrders">The mapping orders.</param>
        /// <param name="mappingSpecifiedOnly">if set to <c>true</c> [mapping specified only].</param>
        /// <returns></returns>
        public override ConvertHandler GetConvertHandler(Type inputType, Type outputType, object inputObject, object outputObject, Dictionary<string, string> mappingNames, List<string> ignoreList, Dictionary<int, string> mappingOrders, bool mappingSpecifiedOnly)
        {
            //保存目标字段名，目标MemberInfo
            Dictionary<string, string> distToSrc = new Dictionary<string, string>();
            Dictionary<string, MemberInfo> members = new Dictionary<string, MemberInfo>();
            //先查找显式映射
            foreach (string sourceName in mappingNames.Keys)
            {
                MemberInfo targetMember = ConvertorGeneratorHelper.GetMemberInfo(outputType, mappingNames[sourceName]);
                Check.Require(targetMember != null, string.Format("member named {0} could not be found in {1}", mappingNames[sourceName], outputType.FullName));
                distToSrc.Add(mappingNames[sourceName], sourceName);
                members.Add(mappingNames[sourceName], targetMember);
            }
            //如果不是只显式映射
            if (!mappingSpecifiedOnly)
            {
                Dictionary<string, MemberInfo> targetMembers = ConvertorGeneratorHelper.GetMembers(outputType);
                foreach (string targetName in targetMembers.Keys)
                {
                    if (!ignoreList.Contains(targetName) && !distToSrc.ContainsKey(targetName))
                    {
                        distToSrc.Add(targetName, targetName);
                        members.Add(targetName, targetMembers[targetName]);
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

        #endregion

        #region IMemberSetterGenerator Members

        public void SetMemberValue(CodeGenerator generator, ArgBuilder targetObject, string memberName, LocalBuilder memberValue)
        {
            MemberInfo memberInfo = ConvertorGeneratorHelper.GetMemberInfo(targetObject.ArgType, memberName);
            if (memberInfo != null)
            {
                generator.Ldarg(targetObject);
                generator.Ldloc(memberValue);
                generator.StoreMember(memberInfo);
            }
        }

        /// <summary>
        /// Ends the set members.
        /// </summary>
        /// <param name="generator">The generator.</param>
        public void EndSetMembers(CodeGenerator generator)
        {
        }
        #endregion

        #region IMemberSetterGenerator Members

        public void BeginSetMembers(CodeGenerator generator, ArgBuilder targetObject)
        {

        }

        public void EndSetMembers(CodeGenerator generator, ArgBuilder targetObject)
        {

        }

        #endregion

        #region IMemberSetterGenerator Members

        public bool ContainsMember(string name, Type type, object outputObject)
        {
            return ConvertorGeneratorHelper.GetMemberInfo(type, name) != null;
        }

        #endregion
    }
}
