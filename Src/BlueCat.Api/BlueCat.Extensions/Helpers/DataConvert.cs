using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace BlueCat.Extensions.Helpers
{
    /// <summary>
    /// 数据转换
    /// </summary>
    public class DataConvert<T> where T : new()
    {
        #region DataTable转List

        /// <summary>
        /// DataTable转List
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<T> ConvertToModel(DataTable dt)
        {
            // 定义集合    
            List<T> ts = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            //获得此模型的公共属性
            PropertyInfo[] propertys = type.GetProperties();
            string tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;// 检查DataTable是否包含此列    

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite)
                        {
                            continue;
                        }

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            switch (pi.PropertyType.FullName)
                            {
                                case "System.Int64":
                                    long _long;
                                    long.TryParse(value.ToString(), out _long);
                                    pi.SetValue(t, _long, null);
                                    break;
                                case "System.Int32":
                                    int _int;
                                    int.TryParse(value.ToString(), out _int);
                                    pi.SetValue(t, _int, null);
                                    break;
                                case "System.Int16":
                                    pi.SetValue(t, System.Convert.ToInt16(value), null);
                                    break;
                                case "System.Boolean":
                                    bool _bool;
                                    bool.TryParse(value.ToString(), out _bool);
                                    pi.SetValue(t, _bool, null);
                                    break;
                                case "System.Double":
                                    double _double;
                                    double.TryParse(value.ToString(), out _double);
                                    pi.SetValue(t, _double, null);
                                    break;
                                case "System.String":
                                    pi.SetValue(t, System.Convert.ToString(value), null);
                                    break;
                                case "System.Decimal":
                                    decimal _decimal;
                                    decimal.TryParse(value.ToString(), out _decimal);
                                    pi.SetValue(t, _decimal, null);
                                    break;
                                case "System.Byte":
                                    byte _byte;
                                    byte.TryParse(value.ToString(), out _byte);
                                    pi.SetValue(t, _byte, null);
                                    break;
                                case "System.Char":
                                    char _char;
                                    char.TryParse(value.ToString(), out _char);
                                    pi.SetValue(t, _char, null);
                                    break;
                                case "System.DateTime":
                                    DateTime time;
                                    DateTime.TryParse(value.ToString(), out time);
                                    pi.SetValue(t, time, null);
                                    break;
                                default:
                                    pi.SetValue(t, value, null);
                                    break;
                            }
                        }
                    }
                }
                ts.Add(t);
            }
            return ts;
        }

        #endregion

        #region List集合转DataTable

        /// <summary>  
        /// List集合转DataTable
        /// </summary>
        /// <param name="list">传入集合</param>  
        /// <param name="isStoreDB">是否存入数据库DateTime字段，date时间范围没事，取出展示不用设置TRUE</param>  
        /// <returns>返回datatable结果</returns>  
        public static DataTable ListToTable(List<T> list, bool isStoreDB = true)
        {
            Type tp = typeof(T);
            PropertyInfo[] proInfos = tp.GetProperties();
            DataTable dt = new DataTable();
            foreach (var item in proInfos)
            {
                dt.Columns.Add(item.Name, item.PropertyType); //添加列明及对应类型  
            }
            foreach (var item in list)
            {
                DataRow dr = dt.NewRow();
                foreach (var proInfo in proInfos)
                {
                    object obj = proInfo.GetValue(item);
                    if (obj == null)
                    {
                        continue;
                    }
                    if (isStoreDB && proInfo.PropertyType == typeof(DateTime) && System.Convert.ToDateTime(obj) < System.Convert.ToDateTime("1753-01-01"))
                    {
                        continue;
                    }
                    dr[proInfo.Name] = obj;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        #endregion

        #region Class To Dictionary

        /// <summary>
        /// Class To Dictionary
        /// 不能转换类里面有字段类型为Class的类型的类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ClassToDictionary(T data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        } 

        #endregion
    }
}
