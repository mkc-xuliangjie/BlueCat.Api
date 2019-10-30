using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BlueCat.ORM.Mapping
{
    public class DataReaderToObjectMapper<T> where T : class
    {
        public DataReaderToObjectMapper()
        {

        }
        /// <summary>
        /// Convert the specified inputObject to the output mapping type.
        /// </summary>
        /// <param name="inputObject">The inputObject must be consistent with the specified inputMappingType in ObjectMapper's Constructor.</param>
        /// <param name="customMappingHandlers"></param>
        /// <returns></returns>
        public T ConvertObject(object inputObject)
        {

            return ConvertObject(inputObject);
        }


        private ObjectMapper mainMapper;
        /// <summary>
        /// Convert the specified inputObject to the output mapping type.
        /// </summary>
        /// <param name="inputObject">The inputObject must be consistent with the specified inputMappingType in ObjectMapper's Constructor.</param>
        /// <returns></returns>
        public T ConvertObject(IDataReader inputObject)
        {
            Type outputType = typeof(T);


            if (mainMapper == null)
            {
                mainMapper = GetObjectMapper(inputObject);
            }

            var outputObject = mainMapper.ConvertObject(inputObject);
          
            return (T)outputObject;

        }

        private ObjectMapper GetObjectMapper(IDataReader dr)
        {
            var objMapper = new ObjectMapper(typeof(IDataReader), typeof(T));
            var sTable = dr.GetSchemaTable();   //不同的DR,可能获取的列有多，有少， 所以按照结构
            var cols = new List<string>();
            foreach (DataRow item in sTable.Rows)
            {
                cols.Add(item[0].ToString());
            }
            foreach (var item in cols.OrderBy(p => p))
            {
                objMapper.AddCustomMappingName(item, item);
            }
            return objMapper;
        }

    }
}
