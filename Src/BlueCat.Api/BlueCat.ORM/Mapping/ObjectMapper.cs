//#define StepPerformance
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;

namespace BlueCat.ORM.Mapping
{
    public interface ICustomObjectMemberMapping
    {
        void Mapping(object inputObject, object outputObject);
    }

    /// <summary>
    /// ObjectMapper
    /// </summary>
    public class ObjectMapper
    {
        #region Private Members

        private Type inputType;
        private Type outputType;
        private Type initType;
        private Dictionary<string, string> mappingNames = new Dictionary<string, string>();

        private Dictionary<int, string> mappingOrders = new Dictionary<int, string>();
        //忽略的目标字段名
        private List<string> ignoreNames = new List<string>();
        private List<ICustomObjectMemberMapping> customMappings = new List<ICustomObjectMemberMapping>();
        private bool mappingSpecifiedOnly = false;

        private DataTable cachedTable;

        #endregion

        /// <summary>
        /// Initialize an instance of ObjectMapper
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="outputType"></param>
        public ObjectMapper(Type inputType, Type outputType)
        {
            Check.Require(inputType, "inputType");
            Check.Require(outputType, "outputType");
            Check.Require(!inputType.IsAssignableFrom(typeof(DataTable)), "intput type could not be DataTable,pls use datarow instead.");
            Check.Require(!outputType.IsAssignableFrom(typeof(DataTable)), "output type could not be DataTable,pls use datarow instead.");
            Check.Require(!outputType.IsAssignableFrom(typeof(IDataReader)), "output type could not be IDataReader.");


            this.inputType = inputType;
            this.outputType = outputType;
            this.initType = outputType;

        }

        public ObjectMapper(Type inputType, Type outputType, Type initType)
            : this(inputType, outputType)
        {
            if (initType != null)
            {
                this.initType = initType;
            }
        }

        public ObjectMapper(Type inputType, Type outputType, Type initType,
            bool mappingSpecifiedOnly)
            : this(inputType, outputType, initType)
        {
            this.mappingSpecifiedOnly = mappingSpecifiedOnly;
        }

        #region mapping Config

        /// <summary>
        /// AddCustomMappingName
        /// </summary>
        /// <param name="inputMemberName"></param>
        /// <param name="outputMemberName"></param>
        public void AddCustomMappingName(string inputMemberName, string outputMemberName)
        {
            Check.Require(inputMemberName, "inputMemberName", Check.NotNullOrEmpty);
            Check.Require(outputMemberName, "outputMemberName", Check.NotNullOrEmpty);

            lock (mappingNames)
            {
                if (mappingNames.ContainsKey(inputMemberName))
                {
                    mappingNames[inputMemberName] = outputMemberName;
                }
                else
                {
                    mappingNames.Add(inputMemberName, outputMemberName);
                }
            }
        }

        /// <summary>
        /// ClearCustomMappingName
        /// </summary>
        public void ClearCustomMappingName()
        {
            lock (mappingNames)
            {
                mappingNames.Clear();
            }
        }

        /// <summary>
        /// RemoveCustomMappingName
        /// </summary>
        /// <param name="inputMemberName"></param>
        public void RemoveCustomMappingName(string inputMemberName)
        {
            Check.Require(inputMemberName, "inputMemberName", Check.NotNullOrEmpty);

            lock (mappingNames)
            {
                mappingNames.Remove(inputMemberName);
            }
        }

        /// <summary>
        /// AddIgnoreMappingName
        /// <remark>abu 2007-11-16 10:53</remark>
        /// </summary>
        /// <param name="outputMemberName">Name of the output member.</param>
        public void AddIgnoreMappingName(string outputMemberName)
        {
            Check.Require(outputMemberName, "outputMemberName", Check.NotNullOrEmpty);
            ignoreNames.Add(outputMemberName);
        }
        /// <summary>
        /// ClearIgnoreMappingName
        /// 	<remark>abu 2007-11-16 10:54</remark>
        /// </summary>
        public void ClearIgnoreMappingName()
        {
            ignoreNames.Clear();
        }
        /// <summary>
        /// RemoveIgnoreMappingName
        /// <remark>abu 2007-11-16 10:54 </remark>
        /// </summary>
        /// <param name="outputMemberName">Name of the output member.</param>
        public void RemoveIgnoreMappingName(string outputMemberName)
        {
            Check.Require(outputMemberName, "outputMemberName", Check.NotNullOrEmpty);
            ignoreNames.Remove(outputMemberName);
        }

        /// <summary>
        /// 	AddCustomMappingFunction
        /// </summary>
        public void AddCustomMapping(ICustomObjectMemberMapping customHandler)
        {
            Check.Require(customHandler, "customHandler", Check.NotNull);
            customMappings.Add(customHandler);
        }

        /// <summary>
        /// 	ClearCustomMappingFunction
        /// </summary>
        public void ClearCustomMappingFunction()
        {
            customMappings.Clear();
        }

        /// <summary>
        /// AddMappingOrder
        /// <remark>abu 2007-11-16 11:00</remark>
        /// </summary>
        /// <param name="outputMemberName">Name of the output member.</param>
        /// <param name="order">The order.</param>
        public void AddMappingOrder(string outputMemberName, int order)
        {
            Check.Require(order, "order", Check.GreaterThan<int>(0));
            Check.Require(outputMemberName, "outputMemberName", Check.NotNullOrEmpty);
            mappingOrders.Add(order, outputMemberName);
        }

        /// <summary>
        /// 	ClearMappingOrder
        /// </summary>
        public void ClearMappingOrder()
        {
            mappingOrders.Clear();
        }

        #endregion

        /// <summary>
        /// Convert the specified inputObject to the output mapping type.
        /// </summary>
        /// <param name="inputObject">The inputObject must be consistent with the specified inputMappingType in ObjectMapper's Constructor.</param>
        /// <param name="customMappingHandlers"></param>
        /// <returns></returns>
        public object ConvertObject(object inputObject)
        {
            Check.Require(inputObject, "inputObject");

            return ConvertObject(inputObject, null);
        }


        ConvertHandler convertHandler;

        /// <summary>
        /// Convert the specified inputObject to the output mapping type.
        /// </summary>
        /// <param name="inputObject">The inputObject must be consistent with the specified inputMappingType in ObjectMapper's Constructor.</param>
        /// <param name="outputObject">outputObject is an instance to set member values on instead of create a new one.</param>
        /// <param name="customMappingHandlers"></param>
        /// <returns></returns>
        public object ConvertObject(object inputObject, object outputObject)
        {
            Check.Require(inputObject, "inputObject");

            if (outputObject == null)
            {
                if (!initType.IsAssignableFrom(typeof(DataRow)))
                    outputObject = Activator.CreateInstance(initType);
                else
                {
                    if (cachedTable != null)
                    {
                        outputObject = cachedTable.Clone().NewRow();
                    }
                }

            }
            if (convertHandler == null)
            {
                lock (this)
                {
                    if (convertHandler == null)
                    {
                        convertHandler = CovnertHandlerFactory.GetConvertHandler(inputType, outputType, inputObject, outputObject, mappingNames, ignoreNames, mappingOrders, mappingSpecifiedOnly);
                    }
                }
            }
            outputObject = convertHandler(inputObject, outputObject);
            if (outputType.IsAssignableFrom(typeof(DataRow)))
            {
                DataTable table = ((DataRow)outputObject).Table;
                if (cachedTable == null)
                {
                    lock (this)
                    {
                        if (cachedTable == null)
                        {
                            cachedTable = table.Clone();
                        }
                    }
                }
            }
            foreach (ICustomObjectMemberMapping customMapping in customMappings)
            {
                customMapping.Mapping(inputObject, outputObject);
            }
            return outputObject;
        }
    }
}
