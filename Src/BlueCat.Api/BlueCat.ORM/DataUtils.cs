using System;
using System.Collections.Generic;
using System.Data;

namespace BlueCat.ORM
{
    /// <summary>
    /// 数据工具类
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-10-18</para>
    /// </remarks>
    public sealed class DataUtils
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataUtils"/> class.
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-18</para>
        /// </remarks>
        public DataUtils() { }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="dbType">Type of the 数据库.</param>
        /// <param name="val">The 值.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-18</para>
        /// </remarks>
        public static string ToString(DbType dbType, object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return "NULL";
            }

            Type type = val.GetType();

            if (dbType == DbType.AnsiString || dbType == DbType.AnsiStringFixedLength)
            {
                return string.Format("'{0}'", val.ToString().Replace("'", "''"));
            }
            else if (dbType == DbType.String || dbType == DbType.StringFixedLength)
            {
                return string.Format("N'{0}'", val.ToString().Replace("'", "''"));
            }
            else if (dbType == DbType.DateTime)
            {
                return string.Format("'{0}'", ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else if (type == typeof(DateTime) || type == typeof(Guid))
            {
                return string.Format("'{0}'", val);
            }
            else if (type== typeof(TimeSpan))
            {
                DateTime baseTime = new DateTime(2006, 11, 23);
                return string.Format("(CAST('{0}' AS datetime) - CAST('{1}' AS datetime))", baseTime + ((TimeSpan)val), baseTime);
            }
            else if (type == typeof(bool))
            {
                return ((bool)val) ? "1" : "0";
            }
            else if (type == typeof(byte[]))
            {
                return "0x" + BitConverter.ToString((byte[])val).Replace("-", string.Empty);
            }
            else if (val is ExpressionClip)
            {
                return ToString((ExpressionClip)val) ;
            }
            else if (type.IsEnum)
            {
                return Convert.ToInt32(val).ToString();
            }
            else if (type.IsValueType)
            {
                return val.ToString();
            }
            else
            {
                return string.Format("'{0}'", val.ToString().Replace("'", "''"));
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-18</para>
        /// </remarks>
        public static string ToString(IExpression expr)
        {
            if (expr == null)
            {
                return null;
            }

            string sql = expr.ToString();

            if (!string.IsNullOrEmpty(sql))
            {
                Dictionary<string, KeyValuePair<DbType, object>>.Enumerator en = expr.Parameters.GetEnumerator();

                while (en.MoveNext())
                {
                    sql = sql.Replace('@' + en.Current.Key, ToString(en.Current.Value.Key, en.Current.Value.Value));
                }
            }

            return sql;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="cmd">The 命令.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-18</para>
        /// </remarks>
        public static string ToString(System.Data.Common.DbCommand cmd)
        {
            if (cmd == null)
            {
                return null;
            }

            string sql = cmd.CommandText;

            if (!string.IsNullOrEmpty(sql))
            {
                System.Collections.IEnumerator en = cmd.Parameters.GetEnumerator();

                while (en.MoveNext())
                {
                    System.Data.Common.DbParameter p = (System.Data.Common.DbParameter)en.Current;
                    sql = sql.Replace(p.ParameterName, ToString(p.DbType, p.Value));
                }
            }

            return sql;
        }
    }
}
