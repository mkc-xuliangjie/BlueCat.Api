using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BlueCat.ORM.Mapping
{
    public class CovnertHandlerFactory
    {
        static Dictionary<Type, Type> getterGenerators = new Dictionary<Type, Type>();
        static Dictionary<Type, Type> setterGenerators = new Dictionary<Type, Type>();
        static Dictionary<string, ConvertHandler> cachedConvertHandler = new Dictionary<string, ConvertHandler>();
        public static ConvertHandler GetConvertHandler(Type inputType, Type outputType, object inputObject, object outputObject, Dictionary<string, string> mappingNames, List<string> ignoreList, Dictionary<int, string> mappingOrders, bool mappingSpecifiedOnly)
        {
            string cachedKey = GetCachedKey(inputType, outputType, mappingNames, ignoreList, mappingOrders, mappingSpecifiedOnly);
            if (!cachedConvertHandler.ContainsKey(cachedKey))
            {
                lock (cachedConvertHandler)
                {
                    if (!cachedConvertHandler.ContainsKey(cachedKey))
                    {
                        Type getterGeneratorType = GetMemberGetterGenerator(inputType);
                        Type setterGeneratorType = GetMemberSetterGenerator(outputType);

                        IMemberGetterGenerator getterGenerator = (IMemberGetterGenerator)Activator.CreateInstance(getterGeneratorType);
                        IMemberSetterGenerator setterGenerator = (IMemberSetterGenerator)Activator.CreateInstance(setterGeneratorType);
                        GeneratorDriver driver = null;
                        if (typeof(GeneratorDriver).IsAssignableFrom(setterGeneratorType))
                        {
                            driver = (GeneratorDriver)Activator.CreateInstance(setterGeneratorType, getterGenerator, setterGenerator);
                        }
                        else
                        {
                            driver = (GeneratorDriver)Activator.CreateInstance(getterGeneratorType, getterGenerator, setterGenerator);
                        }
                        cachedConvertHandler.Add(cachedKey, driver.GetConvertHandler(inputType, outputType, inputObject, outputObject, mappingNames, ignoreList, mappingOrders, mappingSpecifiedOnly));
                    }
                }
            }
            return cachedConvertHandler[cachedKey];
        }
        private static string GetCachedKey(Type inputType, Type outputType, Dictionary<string, string> mappingNames, List<string> ignoreList, Dictionary<int, string> mappingOrders, bool mappingSpecifiedOnly)
        {
            StringBuilder key = new StringBuilder("InputType:" + inputType.FullName + "OutputType:" + outputType.FullName);
            key.AppendFormat("mappingNames:{0}", CompositeDictionary<string, string>(mappingNames));
            key.AppendFormat("ignoreList:{0}", string.Join(",", ignoreList.ToArray()));
            key.AppendFormat("mappingOrders:{0}", CompositeDictionary<int, string>(mappingOrders));
            key.AppendFormat("mappingSpecifiedOnly:{0};", mappingSpecifiedOnly.ToString());
            return key.ToString();
        }
        private static string CompositeDictionary<Key, Value>(Dictionary<Key, Value> dic)
        {
            StringBuilder key = new StringBuilder();
            foreach (Key k in dic.Keys)
            {
                key.AppendFormat("key:{0};value:{1};", k.ToString(), dic[k].ToString());
            }
            return key.ToString();
        }
        public static Type GetMemberGetterGenerator(Type objType)
        {
            Check.Require(objType, "objType");
            if (getterGenerators.ContainsKey(objType))
            {
                return getterGenerators[objType];
            }
            if (typeof(DataRow).IsAssignableFrom(objType) || typeof(DataTable).IsAssignableFrom(objType))
            {
                return typeof(DataRowGetterGenerator);
            }
            if (typeof(IDataReader).IsAssignableFrom(objType))
            {
                return typeof(DataReaderGetterGenerator);
            }
            if (typeof(NameValueCollection).IsAssignableFrom(objType))
            {
                return typeof(NameValueGetterGenerator);
            }
            if (typeof(IDictionary).IsAssignableFrom(objType))
            {
                return typeof(DictionaryGetterGenerator);
            }
            return typeof(ObjectGetterGenerator);
           
        }

        public static void RegisterGetterGenerator(Type objType, Type generatorType)
        {
            Check.Require(objType, "objType");
            Check.Require(generatorType, "generatorType");

            lock (getterGenerators)
            {
                if (getterGenerators.ContainsKey(objType))
                {
                    getterGenerators[objType] = generatorType;
                }
                else
                {
                    getterGenerators.Add(objType, generatorType);
                }
            }
        }

        public static Type GetMemberSetterGenerator(Type objType)
        {
            Check.Require(objType, "objType");
            if (setterGenerators.ContainsKey(objType))
            {
                return setterGenerators[objType];
            }
            if (typeof(DataRow).IsAssignableFrom(objType))
            {
                return typeof(DataRowSetterGenerator);
            }
            if (typeof(NameValueCollection).IsAssignableFrom(objType))
            {
                return typeof(NameValueSetterGenerator);
            }
            if (typeof(IDictionary).IsAssignableFrom(objType))
            {
                return typeof(DictionarySetterGenerator);
            }
            return typeof(ObjectSetterGenerator);
            //if (typeof(IDataReader).IsAssignableFrom(objType))
            //{
            //    return DataReaderMemberGetter.Instance;
            //}
            //else if (typeof(DataRow).IsAssignableFrom(objType) || typeof(DataTable).IsAssignableFrom(objType))
            //{
            //    return DataRowMemberGetter.Instance;
            //}
            //else if (typeof(NameValueCollection).IsAssignableFrom(objType))
            //{
            //    return NameValueCollectionGetter.Instance;
            //}
        }

        public static void RegisterSetterGenerator(Type objType, Type generatorType)
        {
            Check.Require(objType, "objType");
            Check.Require(generatorType, "generatorType");

            lock (setterGenerators)
            {
                if (setterGenerators.ContainsKey(objType))
                {
                    setterGenerators[objType] = generatorType;
                }
                else
                {
                    setterGenerators.Add(objType, generatorType);
                }
            }
        }
    }
}
