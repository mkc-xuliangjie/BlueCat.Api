// ***********************************************************************
// Assembly         : BlueCat.ORM
// Author           : Teddy
// Created          : 2018-03-22
//
// Last Modified By : Teddy
// Last Modified On : 2018-03-22
// ***********************************************************************
// <copyright file="CustomSqlSection.cs" company="上海同是科技股份有限公司">
//     Copyright (c) 上海同是科技股份有限公司. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BlueCat.ORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{
    /// <summary>
    /// 自定义脚本
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2018/3/22</para>
    /// </remarks>
    public sealed class CustomSqlSection
    {
        #region Private Members

        /// <summary>
        /// The database
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private readonly DataContext db;

        /// <summary>
        /// The tran
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private DbTransaction tran;
        /// <summary>
        /// The input parameter names
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private List<string> inputParamNames = new List<string>();
        /// <summary>
        /// The input parameter types
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private List<DbType> inputParamTypes = new List<DbType>();
        /// <summary>
        /// The input parameter values
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private List<object> inputParamValues = new List<object>();

        /// <summary>
        /// The skip count
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private int skipCount;
        /// <summary>
        /// The take count
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private int takeCount;
        /// <summary>
        /// The order by
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private string orderBy;


        /// <summary>
        /// Gets or sets the SQL.
        /// </summary>
        /// <value>The SQL.</value>
        /// <exception cref="Exception">分页时必须设置OrderBy</exception>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        internal string SQL
        {
            get
            {
                if (takeCount > 0)
                {
                    if (skipCount < 0) skipCount = 0;
                    if (string.IsNullOrEmpty(orderBy)) throw new Exception("分页时必须设置OrderBy");

                    var sb = new StringBuilder();
                    sb.Append("WITH [__T2] AS (");
                    sb.AppendFormat("SELECT [__T].* ,ROW_NUMBER() OVER (ORDER BY {0}) AS [__Pos] FROM ( ", orderBy);
                    sb.Append(sql);
                    sb.Append(") AS [__T] )");
                    sb.AppendFormat(" SELECT * FROM [__T2] WHERE [__Pos]>{0} AND [__Pos]<={1}  ", skipCount, skipCount + takeCount);
                    return sb.ToString();
                }
                else
                {
                    return sql;
                }

            }
            set
            {
                sql = value;
            }
        }
        /// <summary>
        /// The SQL
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private string sql;


        /// <summary>
        /// Finds the data reader.
        /// </summary>
        /// <returns>IDataReader.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private IDataReader FindDataReader()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateCustomSqlCommand(SQL, inputParamNames.ToArray(),
                inputParamTypes.ToArray(), inputParamValues.ToArray());
            return tran == null ? db.ExecuteReader(cmd) : db.ExecuteReader(cmd, tran);
        }

        /// <summary>
        /// Finds the data set.
        /// </summary>
        /// <returns>DataSet.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private DataSet FindDataSet()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateCustomSqlCommand(SQL, inputParamNames.ToArray(),
                inputParamTypes.ToArray(), inputParamValues.ToArray());
            return tran == null ? db.ExecuteDataSet(cmd) : db.ExecuteDataSet(cmd, tran);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomSqlSection"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="sql">The SQL.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public CustomSqlSection(DataContext db, string sql)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(sql != null, "sql could not be null.");

            this.db = db;
            this.sql = sql;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// 设置查询参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="type">数据类型</param>
        /// <param name="value">值</param>
        /// <returns>CustomSqlSection.</returns>
        /// <remarks>Context.CustomSql("SELECT * FROM table WHERE name=@name")
        /// .AddInputParameter("@name", DbType.String, "aa")
        /// .ToDataReader();</remarks>
        public CustomSqlSection AddInputParameter(string name, DbType type, object value)
        {
            Check.Require(!string.IsNullOrEmpty(name), "name could not be null or empty!");

            inputParamNames.Add(name);
            inputParamTypes.Add(type);
            inputParamValues.Add(value);

            return this;
        }


        /// <summary>
        /// 设置分页获取数据
        /// </summary>
        /// <param name="skipCount">跳过的记录数</param>
        /// <param name="takeCount">提取的记录数</param>
        /// <returns>CustomSqlSection.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public CustomSqlSection SetSelectRange(int skipCount, int takeCount)
        {
            this.skipCount = skipCount;
            this.takeCount = takeCount;

            return this;

        }

        /// <summary>
        /// 设置排序字段
        /// </summary>
        /// <param name="order">排序字段，示例 "Name DESC" OR "Name ASC"</param>
        /// <returns>CustomSqlSection.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public CustomSqlSection OrderBy(string order)
        {
            this.orderBy = order;

            return this;
        }

        /// <summary>
        /// 分页取数据时用于获取表中的记录总数
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public int GetTotalForPaging()
        {
            var sb = new StringBuilder();
            sb.Append("SELECT Count(*) From (");
            sb.Append(sql);
            sb.Append(") AS [__T]");

            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateCustomSqlCommand(sb.ToString(), inputParamNames.ToArray(),
                 inputParamTypes.ToArray(), inputParamValues.ToArray());
            return int.Parse(db.ExecuteScalar(cmd).ToString());
        }

        /// <summary>
        /// 设置事务
        /// </summary>
        /// <param name="tran">The tran.</param>
        /// <returns>CustomSqlSection.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public CustomSqlSection SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public int ExecuteNonQuery()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateCustomSqlCommand(SQL, inputParamNames.ToArray(),
                inputParamTypes.ToArray(), inputParamValues.ToArray());
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
        }

        /// <summary>
        /// 执行并返回结果
        /// </summary>
        /// <returns>System.Object.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public object ToScalar()
        {
            IDataReader reader = FindDataReader();
            object retObj = null;
            if (reader.Read())
            {
                retObj = reader.GetValue(0);
            }
            reader.Close();
            reader.Dispose();

            return retObj;
        }

        /// <summary>
        /// 执行并返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp">The exp.</param>
        /// <returns>T.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public T ToScalar<T>(Func<IDataReader, T> exp)
        {
            T retObj = default(T);

            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return retObj;

                if (reader.Read())
                    retObj = exp(reader);
                reader.Close();
            }

            return retObj;
        }

        /// <summary>
        /// 执行并返回结果 DataReader
        /// </summary>
        /// <returns>IDataReader.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public IDataReader ToDataReader()
        {
            return FindDataReader();
        }

        /// <summary>
        /// 执行并返回结果 DataSet
        /// </summary>
        /// <returns>DataSet.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public DataSet ToDataSet()
        {
            return FindDataSet();
        }

        /// <summary>
        /// 将结果集转换为指定的数据类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List&lt;T&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public List<T> ToList<T>()
             where T : class
        {
            List<T> retObjs = new List<T>();

            var drToObjectMapper = new DataReaderToObjectMapper<T>();
            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return retObjs;

                var list = new List<T>();
                while (reader.Read())
                {
                    list.Add(drToObjectMapper.ConvertObject(reader));
                }
                retObjs = list;
                reader.Close();
            }

            return retObjs;
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public T ToObject<T>()
             where T : class
        {
            T retObj = default(T);
            var drToObjectMapper = new DataReaderToObjectMapper<T>();
            using (IDataReader reader = this.ToDataReader())
            {

                if (reader.Read())
                    retObj = drToObjectMapper.ConvertObject(reader);
            }

            return retObj;
        }

        /// <summary>
        /// 自定义的返回结果
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="exp">表达式</param>
        /// <returns>List&lt;T&gt;.</returns>
        /// <remarks>使用方法
        /// var result =  [...].ToList(p =&gt; new { P1=p["name"], P2=p["id"] });
        /// result[0].P1   result[0].P2</remarks>
        public List<T> ToList<T>(Func<IDataReader, T> exp)
        {
            List<T> retObjs = new List<T>();

            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return retObjs;

                while (reader.Read())
                {
                    retObjs.Add(exp(reader));
                }
                reader.Close();
            }

            return retObjs;
        }

        #endregion
    }

}
