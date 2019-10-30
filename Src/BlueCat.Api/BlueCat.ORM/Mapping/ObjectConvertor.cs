using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace BlueCat.ORM.Mapping
{
    public static class ObjectConvertor
    {
        #region ToObject


        /// <summary>
        /// 	Convert to outputType
        /// </summary>
        /// <typeparam name="OutputType">The type of the output type.</typeparam>
        /// <param name="inputObject">The input object.</param>
        /// <returns></returns>
        public static OutputType ToObject<OutputType>(object inputObject)
            where OutputType : class
        {
            return (OutputType)ToObject(inputObject.GetType(), typeof(OutputType), typeof(OutputType), inputObject);
        }

        /// <summary>
        /// 	Convert to outputType
        /// </summary>
        /// <typeparam name="InputType">The type of the input type.</typeparam>
        /// <typeparam name="OutputType">The type of the output type.</typeparam>
        /// <param name="inputObject">The input object.</param>
        /// <returns></returns>
        public static OutputType ToObject<InputType, OutputType>(InputType inputObject)
            where InputType : class
            where OutputType : class
        {
            return (OutputType)ToObject(typeof(InputType), typeof(OutputType), typeof(OutputType), inputObject);
        }

        /// <summary>
        /// 	Convert to outputType
        /// </summary>
        /// <typeparam name="InputType">The type of the nput type.</typeparam>
        /// <typeparam name="OutputType">The type of the utput type.</typeparam>
        /// <typeparam name="InitType">The type of the nit type.</typeparam>
        /// <param name="inputObject">The input object.</param>
        /// <returns></returns>
        public static OutputType ToObject<InputType, OutputType, InitType>(InputType inputObject)
            where OutputType : class
            where InitType : class
        {
            return ToObject<InputType, OutputType, InitType>(inputObject, string.Empty);
        }

        public static OutputType ToObject<InputType, OutputType, InitType>(InputType inputObject, string viewName)
        {
            return (OutputType)ToObject(typeof(InputType), typeof(OutputType), typeof(InitType), inputObject, viewName);
        }

        /// <summary>
        /// 	Convert to outputType
        /// </summary>
        /// <typeparam name="OutputType">The type of the output type.</typeparam>
        /// <param name="inputObject">The input object.</param>
        /// <returns></returns>
        public static OutputType ToObject<OutputType>(object inputObject, string viewName)
            where OutputType : class
        {
            return (OutputType)ToObject(inputObject.GetType(), typeof(OutputType), typeof(OutputType), inputObject, viewName);
        }

        /// <summary>
        /// 	Convert to outputType
        /// </summary>
        /// <typeparam name="InputType">The type of the nput type.</typeparam>
        /// <typeparam name="OutputType">The type of the utput type.</typeparam>
        /// <param name="inputObject">The input object.</param>
        /// <returns></returns>
        public static OutputType ToObject<InputType, OutputType>(InputType inputObject, string viewName)
            where InputType : class
            where OutputType : class
        {
            return (OutputType)ToObject(typeof(InputType), typeof(OutputType), typeof(OutputType), inputObject, viewName);
        }

        /// <summary>
        /// 	convert to object
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="initType">Type of the init.</param>
        /// <param name="inputObject">The input object.</param>
        /// <returns></returns>
        public static object ToObject(Type inputType, Type outputType, Type initType, object inputObject)
        {
            return ToObject(inputType, outputType, initType, inputObject, string.Empty);
        }

        /// <summary>
        /// 	convert to object
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="initType">Type of the init.</param>
        /// <param name="inputObject">The input object.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public static object ToObject(Type inputType, Type outputType, Type initType, object inputObject, string viewName)
        {
            return ToObject(inputType, outputType, initType, inputObject, null, viewName);

        }


        /// <summary>
        /// Toes the object.
        /// </summary>
        /// <typeparam name="OutputType">The type of the utput type.</typeparam>
        /// <param name="inputObject">The input object.</param>
        /// <param name="outputObject">The output object.</param>
        /// <returns></returns>
        public static OutputType ToObject<OutputType>(object inputObject, OutputType outputObject)
            where OutputType : class
        {
            return ToObject<OutputType>(inputObject, outputObject, string.Empty);
        }
        /// <summary>
        /// Toes the object.
        /// </summary>
        /// <typeparam name="OutputType">The type of the utput type.</typeparam>
        /// <param name="inputObject">The input object.</param>
        /// <param name="outputObject">The output object.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public static OutputType ToObject<OutputType>(object inputObject, OutputType outputObject, string viewName)
        {
            return (OutputType)ToObject(inputObject.GetType(), typeof(OutputType), inputObject, outputObject, viewName);
        }
        /// <summary>
        /// Toes the object.
        /// </summary>
        /// <typeparam name="InpuType">The type of the npu type.</typeparam>
        /// <typeparam name="OutputType">The type of the utput type.</typeparam>
        /// <param name="inputObject">The input object.</param>
        /// <param name="outputObject">The output object.</param>
        /// <returns></returns>
        public static OutputType ToObject<InpuType, OutputType>(InpuType inputObject, OutputType outputObject)
            where InpuType : class
            where OutputType : class
        {
            return ToObject<InpuType, OutputType>(inputObject, outputObject, string.Empty);
        }
        /// <summary>
        /// Toes the object.
        /// </summary>
        /// <typeparam name="InpuType">The type of the npu type.</typeparam>
        /// <typeparam name="OutputType">The type of the utput type.</typeparam>
        /// <param name="inputObject">The input object.</param>
        /// <param name="outputObject">The output object.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public static OutputType ToObject<InpuType, OutputType>(InpuType inputObject, OutputType outputObject, string viewName)
            where InpuType : class
            where OutputType : class
        {
            return (OutputType)ToObject(typeof(InpuType), typeof(OutputType), inputObject, outputObject, viewName);
        }
        public static object ToObject(Type inputType, Type outputType, object inputObject, object outputObject)
        {
            return ToObject(inputType, outputType, inputObject, outputObject, string.Empty);
        }
        public static object ToObject(Type inputType, Type outputType, object inputObject, object outputObject, string viewName)
        {
            return ToObject(inputType, outputType, null, inputObject, outputObject, viewName);
        }
        private static object ToObject(Type inputType, Type outputType, Type initType, object inputObject, object outputObject, string viewName)
        {
            ObjectMapper objectMapper = GetObjectMapper(inputType, outputType, initType, viewName);
            return objectMapper.ConvertObject(inputObject, outputObject);
        }
        #endregion

        /// <summary>
        /// 	get object mapper
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="initType">Type of the init.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        internal static ObjectMapper GetObjectMapper(Type inputType, Type outputType, Type initType, string viewName)
        {
            ObjectMapper objectMapper = null; //ConfigurationManager.GetObjectMapper(inputType, outputType, viewName);
            if (objectMapper == null)
            {
                objectMapper = new ObjectMapper(inputType, outputType, initType);
            }
            return objectMapper;
        }

        #region ToList


        /// <summary>
        /// convert to object list
        /// </summary>
        /// <typeparam name="InputType">The type of the input type.</typeparam>
        /// <typeparam name="OutputType">The type of the output type.</typeparam>
        /// <param name="inputList">The input list.(IEnumerable or DataTable or DataReader)</param>
        /// <returns></returns>
        public static OutputType[] ToList<InputType, OutputType>(object inputList)
            where InputType : class
            where OutputType : class
        {
            return ToList<InputType, OutputType>(inputList, string.Empty);
        }

        /// <summary>
        /// 	convert to object list
        /// </summary>
        /// <typeparam name="InputType">The type of the nput type.</typeparam>
        /// <typeparam name="OutputType">The type of the utput type.</typeparam>
        /// <typeparam name="InitType">The type of the nit type.</typeparam>
        /// <param name="inputObject">The input object.(IEnumerable or DataTable or DataReader)</param>
        /// <returns></returns>
        public static OutputType[] ToList<InputType, OutputType, InitType>(object inputList)
            where InputType : class
            where OutputType : class
            where InitType : class
        {
            return ToList<InputType, OutputType, InitType>(inputList, string.Empty);
        }

        /// <summary>
        /// 	convert to object list
        /// </summary>
        /// <typeparam name="InputType">The type of the nput type.</typeparam>
        /// <typeparam name="OutputType">The type of the utput type.</typeparam>
        /// <param name="inputObject">The input object.(IEnumerable or DataTable or DataReader)</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public static OutputType[] ToList<InputType, OutputType>(object inputList, string viewName)
            where InputType : class
            where OutputType : class
        {
            return ToList<InputType, OutputType, OutputType>(inputList, viewName);
        }

        /// <summary>
        /// 	convert to list
        /// </summary>
        /// <typeparam name="InputType">The type of the input type.</typeparam>
        /// <typeparam name="OutputType">The type of the output type.</typeparam>
        /// <typeparam name="InitType">The type of the init type.</typeparam>
        /// <param name="inputList">The input list.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public static OutputType[] ToList<InputType, OutputType, InitType>(object inputList, string viewName)
            where InputType : class
            where OutputType : class
            where InitType : class
        {
            return ToList<OutputType>(inputList, typeof(InputType), typeof(InitType), viewName);
        }

        /// <summary>
        /// 	convert to list
        /// </summary>
        /// <typeparam name="OutputType">The type of the output type.</typeparam>
        /// <param name="inputList">The input list.</param>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="initType">Type of the init.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public static OutputType[] ToList<OutputType>(object inputList, Type inputType, Type initType, string viewName)
            where OutputType : class
        {
            List<OutputType> list = new List<OutputType>();
            if (inputList is DataTable)
            {
                ObjectMapper objectMapper = GetObjectMapper(typeof(DataRow), typeof(OutputType), initType, viewName);
                foreach (DataRow dataRow in ((DataTable)inputList).Rows)
                {
                    list.Add((OutputType)objectMapper.ConvertObject(dataRow));
                }
            }
            else if (inputList is IDataReader)
            {
                ObjectMapper objectMapper = GetObjectMapper(typeof(IDataReader), typeof(OutputType), initType, viewName);
                var dr = inputList as IDataReader;

                var sTable = dr.GetSchemaTable();   //不同的DR,可能获取的列有多，有少， 所以按照结构
                var cols = new List<string>();
                foreach (DataRow item in sTable.Rows)
                {
                    cols.Add(item[0].ToString());
                  
                }
                foreach (var item in cols.OrderBy(p=>p))
                {
                    objectMapper.AddCustomMappingName(item, item);
                }
               
                IDataReader dataReader = (IDataReader)inputList;
                while (dataReader.Read())
                {
                    list.Add((OutputType)objectMapper.ConvertObject(dataReader));
                }
            }
            else
            {
                IEnumerable enumerable = (IEnumerable)inputList;
                IEnumerator enumerator = enumerable.GetEnumerator();
                ObjectMapper objectMapper = GetObjectMapper(inputType, typeof(OutputType), initType, viewName);
                while (enumerator.MoveNext())
                {
                    list.Add((OutputType)objectMapper.ConvertObject(enumerator.Current));
                }
            }
            return list.ToArray();
        }


        /// <summary>
        /// 	ToList
        /// </summary>
        /// <param name="inputList">The input list.</param>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <returns></returns>
        public static object[] ToList(object inputList, Type inputType, Type outputType)
        {
            return ToList(inputList, inputType, outputType, string.Empty);
        }
        /// <summary>
        /// 	ToList
        /// </summary>
        /// <param name="inputList">The input list.</param>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public static object[] ToList(object inputList, Type inputType, Type outputType, string viewName)
        {
            return ToList(inputList, inputType, outputType, viewName);
        }
        /// <summary>
        /// 	ToList
        /// </summary>
        /// <param name="inputList">The input list.</param>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="initType">Type of the init.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public static object[] ToList(object inputList, Type inputType, Type outputType, Type initType, string viewName)
        {
            ArrayList list = new ArrayList();
            if (inputList is DataTable)
            {
                ObjectMapper objectMapper = GetObjectMapper(typeof(DataRow), outputType, initType, viewName);
                foreach (DataRow dataRow in ((DataTable)inputList).Rows)
                {
                    list.Add(objectMapper.ConvertObject(dataRow));
                }
            }
            else if (inputList is IDataReader)
            {
                ObjectMapper objectMapper = GetObjectMapper(typeof(IDataReader), outputType, initType, viewName);
                IDataReader dataReader = (IDataReader)inputList;
                var dr = inputList as IDataReader;

                var sTable = dr.GetSchemaTable();   //不同的DR,可能获取的列有多，有少， 所以按照结构
                var cols = new List<string>();
                foreach (DataRow item in sTable.Rows)
                {
                    cols.Add(item[0].ToString());

                }
                foreach (var item in cols.OrderBy(p => p))
                {
                    objectMapper.AddCustomMappingName(item, item);
                }
                while (dataReader.Read())
                {
                    list.Add(objectMapper.ConvertObject(dataReader));
                }
            }
            else
            {
                IEnumerable enumerable = (IEnumerable)inputList;
                IEnumerator enumerator = enumerable.GetEnumerator();
                ObjectMapper objectMapper = GetObjectMapper(inputType, outputType, initType, viewName);
                while (enumerator.MoveNext())
                {
                    list.Add(objectMapper.ConvertObject(enumerator.Current));
                }
            }
            return list.ToArray();
        }
        #endregion

        #region ToDataTable

        /// <summary>
        /// 	convert to DataTable
        /// </summary>
        /// <param name="inputList">The input list.</param>
        /// <param name="inputType">Type of the input.</param>
        /// <returns></returns>
        public static DataTable ToDataTable(object inputList, Type inputType)
        {
            return ToDataTable(inputList, inputType, DataRowState.Unchanged);
        }
        public static DataTable ToDataTable(object inputList, Type inputType, DataRowState rowState)
        {
            return ToDataTable(inputList, inputType, string.Empty, rowState);
        }
        /// <summary>
        /// 	convert to DataTable
        /// </summary>
        /// <param name="inputList">The input list.</param>
        /// <param name="inputType">Type of the input.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
        public static DataTable ToDataTable(object inputList, Type inputType, string viewName, DataRowState rowState)
        {
            DataTable dataTable = null;
            //if (inputList is DataTable)
            //{
            //    ObjectMapper objectMapper = GetObjectMapper(typeof(DataRow), typeof(DataTable), null, viewName);
            //    foreach (DataRow dataRow in ((DataTable)inputList).Rows)
            //    {
            //        dataTable = (DataTable)objectMapper.ConvertObject(dataRow, dataTable);
            //    }
            //}
            //else if (inputList is IDataReader)
            //{
            //    ObjectMapper objectMapper = GetObjectMapper(typeof(IDataReader), typeof(DataTable), null, viewName);
            //    IDataReader dataReader = (IDataReader)inputList;
            //    while (dataReader.Read())
            //    {
            //        dataTable = (DataTable)objectMapper.ConvertObject(dataReader, dataTable);
            //    }
            //}
            //else
            //{
            Check.Require(inputList is IEnumerable, "input list type must implement from IEnumerable");
            IEnumerable enumerable = (IEnumerable)inputList;
            IEnumerator enumerator = enumerable.GetEnumerator();
            ObjectMapper objectMapper = GetObjectMapper(inputType, typeof(DataRow), null, viewName);
            DataRow dataRow = null;
            while (enumerator.MoveNext())
            {
                dataRow = (DataRow)objectMapper.ConvertObject(enumerator.Current, dataRow);
                dataTable = dataRow.Table;
                dataTable.Rows.Add(dataRow);

                switch (rowState)
                {
                    case DataRowState.Added:
                        break;
                    case DataRowState.Deleted:
                        break;
                    case DataRowState.Detached:
                        break;
                    case DataRowState.Modified:
                        dataRow.AcceptChanges();
                        dataRow.SetModified();
                        break;
                    case DataRowState.Unchanged:
                        dataRow.AcceptChanges();
                        break;
                    default:
                        break;
                }

                dataRow = dataTable.NewRow();
            }
            //}

            //dataTable.AcceptChanges();
            return dataTable;
        }

        #endregion
    }
}
