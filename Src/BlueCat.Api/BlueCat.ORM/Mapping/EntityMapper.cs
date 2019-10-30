using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection.Emit;
using System.Reflection;

namespace BlueCat.ORM.Mapping
{
    public class EntityMapper<T, TSchema>
        where T : EntityBase
        where TSchema : TableSchema
    {
        public EntityMapper(Dictionary<string, TableSchema> exTable)
        {
            this.exProperty = exTable;
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


        private ObjectMapper GetExtPropertyMapper(Type type)
        {
            if (!extPropertyMapper.ContainsKey(type))
            {
                extPropertyMapper.Add(type, new ObjectMapper(typeof(IDataReader), type));
            }
            return extPropertyMapper[type];
        }
        private Dictionary<Type, ObjectMapper> extPropertyMapper = new Dictionary<Type, ObjectMapper>();


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

            foreach (string propertyName in exProperty.Keys)
            {
                GetExObj(propertyName, outputObject, outputType, inputObject);
            }
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


        private object GetExObj(string path, object parentObj, Type parentType, IDataReader inputObject)
        {
            var t = path.IndexOf('.');

            if (t < 0)
            {
                var propertyName = path;
                var property = parentType.GetProperty(propertyName);
                var objMapper = GetExtPropertyMapper(property.PropertyType);
                var table = exProperty[propertyName];
                foreach (QueryColumn item in table.GetColumns())
                {
                    objMapper.AddCustomMappingName(item.Name.Replace(".", "_"), item.Name.Split('.')[1]);
                }
                var obj = objMapper.ConvertObject(inputObject);
                property.SetValue(parentObj, obj, null);
                return parentObj;
            }
            else
            {
                var propertyName = path.Substring(0, t + 1);
                var property = parentType.GetProperty(propertyName);
                var objMapper = GetExtPropertyMapper(property.PropertyType);
                var table = exProperty[propertyName];
                foreach (QueryColumn item in table.GetColumns())
                {
                    objMapper.AddCustomMappingName(item.Name, item.Name.Split('.')[1]);
                }
                var obj = objMapper.ConvertObject(inputObject);
                property.SetValue(parentObj, obj, null);

                path = path.Remove(0, t + 1);
                return GetExObj(path, obj, property.ReflectedType, inputObject);
            }
        }

        private Dictionary<string, TableSchema> exProperty;
    }
}
