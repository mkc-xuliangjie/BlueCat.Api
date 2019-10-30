using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Reflection;
using BlueCat.ORM.Mapping;
using System.Linq;

namespace BlueCat.ORM
{
    public sealed class SelectSqlSection<T>
        where T : EntityBase
    {
        #region Private Members

        private readonly DataContext db;
        private readonly string tableName;
        private string[] columnNames = new string[] { "*" };
        private DbTransaction tran;
        private WhereClip whereClip;
        private int topItemCount = int.MaxValue;
        private int skipItemCount = 0;
        private string identyColumnName;
        private bool identyColumnIsNumber = false;
        private TableSchema table;
        private Dictionary<string, TableSchema> includeTables = new Dictionary<string, TableSchema>();

        private DbCommand PrepareCommand()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateSelectRangeCommand(whereClip, columnNames, topItemCount,
                skipItemCount, identyColumnName, identyColumnIsNumber);
            if (cmd != null)
            {
                string topDistinctPrefix = "SELECT TOP " + this.topItemCount.ToString() + " DISTINCT";
                if (this.topItemCount > 0 && this.skipItemCount == 0 && cmd.CommandText.StartsWith(topDistinctPrefix))
                {
                    cmd.CommandText = cmd.CommandText.Replace(topDistinctPrefix, "SELECT DISTINCT TOP " + this.topItemCount.ToString());
                }
            }
            return cmd;
        }

        #endregion

        #region Constructors

        public SelectSqlSection(DataContext db, TableSchema table, params ExpressionClip[] columns)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(table != null, "table could not be null.");

            this.db = db;
            this.table = table;
            this.tableName = table.GetTableName();

            this.whereClip = new WhereClip(new FromClip(table));

            if (columns != null && columns.Length > 0)
            {
                this.columnNames = new string[columns.Length];
                for (int i = 0; i < columns.Length; ++i)
                {
                    this.columnNames[i] = columns[i].ToString();
                }
            }
        }

        public SelectSqlSection(DataContext db, TableSchema table, WhereClip whereClip, params ExpressionClip[] columns)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(table != null, "table could not be null.");

            this.db = db;
            this.table = table;
            this.tableName = table.GetTableName();

            this.whereClip = whereClip;

            if (columns != null && columns.Length > 0)
            {
                this.columnNames = new string[columns.Length];
                for (int i = 0; i < columns.Length; ++i)
                {
                    this.columnNames[i] = columns[i].ToString();
                }
            }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// 设置事务
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        public SelectSqlSection<T> SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        /// <summary>
        /// 设置查询的条件
        /// </summary>
        /// <param name="where">lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <returns></returns>
        public SelectSqlSection<T> Where(WhereClip where)
        {
            whereClip.And(where);
            return this;
        }


        public int GetTotalForPaging()
        {
            var where = this.whereClip.Clone() as WhereClip;
            where.OrderBy = string.Empty;
            return (int)new SelectSqlSection<T>(db, table, where, table.Count).ToScalar();
        }


        /// <summary>
        /// 设置排序的字段
        /// </summary>
        /// <param name="orderBys">排序的字段</param>
        /// <returns></returns>
        public SelectSqlSection<T> OrderBy(params OrderByClip[] orderBys)
        {
            if (orderBys != null && orderBys.Length > 0)
            {
                if (orderBys.Length == 1)
                {
                    whereClip.SetOrderBy(orderBys[0].OrderBys.ToArray());
                }
                else
                {
                    OrderByClip combinedOrderBy = new OrderByClip();
                    for (int i = 0; i < orderBys.Length; ++i)
                    {
                        combinedOrderBy = combinedOrderBy & orderBys[i];
                    }
                    whereClip.SetOrderBy(combinedOrderBy.OrderBys.ToArray());
                }
            }

            return this;
        }

        private List<OrderByClip> orderByClips = new List<OrderByClip>();

        public SelectSqlSection<T> GroupBy(params QueryColumn[] columns)
        {
            if (columns != null && columns.Length > 0)
            {
                string[] columnNames = new string[columns.Length];
                for (int i = 0; i < columns.Length; ++i)
                {
                    columnNames[i] = columns[i].Name;
                }
                this.whereClip.SetGroupBy(columnNames);
            }

            return this;
        }

        public SelectSqlSection<T> Having(WhereClip whereClip)
        {
            this.whereClip.SetHaving(whereClip);
            return this;
        }

        /// <summary>
        /// 设置分组的字段
        /// </summary>
        /// <returns></returns>
        public SelectSqlSection<T> GroupBy(Func<TableSchema, QueryColumn> exp)
        {
            return this.GroupBy(exp(table));
        }

        /// <summary>
        /// 设置分组的字段
        /// </summary>
        /// <param name="exp">排序字段：lambda表达式 如 ： p=> p.Name.Asc 或者 p=>p.Name.Desc</param>
        /// <returns></returns>
        public SelectSqlSection<T> GroupBy(Func<TableSchema, ExpressionClip[]> exp)
        {
            var cols = exp(table);
            var groupCols = cols.Select(p => p as QueryColumn);
            return this.GroupBy(groupCols.ToArray());
        }

        /// <summary>
        /// 设置获取数据的范围
        /// </summary>
        /// <param name="topItemCount"></param>
        /// <param name="skipItemCount"></param>
        /// <returns></returns>
        public SelectSqlSection<T> SetSelectRange(int topItemCount, int skipItemCount)
        {
            Check.Require(topItemCount > 0, "topItemCount MUST > 0");
            Check.Require(skipItemCount >= 0, "skipItemCount MUST >= 0");

            this.topItemCount = topItemCount;
            this.skipItemCount = skipItemCount;
            this.identyColumnName = table.PKColumn.Name;
            this.identyColumnIsNumber =
                (table.PKColumn.DbType == DbType.Int32) ||
                (table.PKColumn.DbType == DbType.Int16) ||
                (table.PKColumn.DbType == DbType.Int64) ||
                (table.PKColumn.DbType == DbType.Byte) ||
                (table.PKColumn.DbType == DbType.Double) ||
                (table.PKColumn.DbType == DbType.Currency) ||
                (table.PKColumn.DbType == DbType.Decimal) ||
                (table.PKColumn.DbType == DbType.SByte) ||
                (table.PKColumn.DbType == DbType.Single) ||
                (table.PKColumn.DbType == DbType.UInt16) ||
                (table.PKColumn.DbType == DbType.UInt32) ||
                (table.PKColumn.DbType == DbType.UInt64);

            return this;
        }

        /// <summary>
        /// 设置获取数据的条数 = topCount
        /// </summary>
        ///<param name="topCount">获取数据的条数</param>
        /// <returns></returns>
        public SelectSqlSection<T> Top(int topCount)
        {
            Check.Require(topCount > 0, "topCount MUST > 0");
            this.topItemCount = topCount;

            return this;
        }

        /// <summary>
        /// 设置获取数据的条数 = 1
        /// </summary>
        /// <returns></returns>
        public SelectSqlSection<T> Top()
        {
            return this.Top(1);
        }


        public SelectSqlSection<T> Include(TableSchema foreignTable)
        {

            if (!foreignTable.IsForeignTable) throw new Exception("这不是一个关联表");

            var cols = new List<string>();
            cols.AddRange(columnNames);
            cols.AddRange(foreignTable.GetComplexColumns());
            this.columnNames = cols.ToArray();

            includeTables.Add(foreignTable.GetPropertyName(), foreignTable);

            return this.Join(foreignTable, foreignTable.JoinWhere);
        }


        /// <summary>
        /// 设置查询结果集的关联表 ： 结果会返回管理表的数据
        /// </summary>
        /// <param name="exp">管理的表达式  p => p.UserType [UserType为外键表]</param>
        /// <returns></returns>
        public SelectSqlSection<T> Include(Func<TableSchema, TableSchema> exp)
        {
            return this.Include(exp(table));
        }


        public SelectSqlSection<T> Join(TableSchema joinTable, WhereClip joinOnWhere)
        {
            Check.Require(joinTable != null, "joinTable could not be null.");
            Check.Require(!WhereClip.IsNullOrEmpty(joinOnWhere), "joinOnWhere could not be null or empty.");

            this.whereClip.From.Join(joinTable, joinOnWhere);

            return this;
        }

        public SelectSqlSection<T> Join<FTable>(FTable fTable, Func<TableSchema, FTable, WhereClip> joinOnWhereExp) where FTable : TableSchema, new()
        {
            var forgeinTable = new FTable();
            return this.Join(forgeinTable, joinOnWhereExp(this.table, forgeinTable));
        }

        /// <summary>
        /// 返回单独结果
        /// </summary>
        /// <returns></returns>
        public object ToScalar()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteScalar(cmd) : db.ExecuteScalar(cmd, tran);
        }

        /// <summary>
        /// 返回一个DataReader结果集
        /// </summary>
        /// <returns></returns>
        public IDataReader ToDataReader()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteReader(cmd) : db.ExecuteReader(cmd, tran);
        }

        /// <summary>
        /// 返回一个DataSet结果集
        /// </summary>
        /// <returns></returns>
        public DataSet ToDataSet()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteDataSet(cmd) : db.ExecuteDataSet(cmd, tran);
        }

        /// <summary>
        /// 返回单独实体
        /// </summary>
        /// <returns></returns>
        public T ToSingleObject()
        {
            T retObj = default(T);

            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return retObj;

                EntityMapper<T, TableSchema> map = new EntityMapper<T, TableSchema>(includeTables);
                if (reader.Read())
                    retObj = map.ConvertObject(reader);
                reader.Close();
            }

            return retObj;
        }


        /// <summary>
        /// 返回单独实体
        /// </summary>
        /// <returns></returns>
        public TResult ToSingleObject<TResult>(Func<IDataReader, TResult> exp)
        {
            TResult retObj = default(TResult);

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
        /// 返回查询的结果集
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            var list = new List<T>();
            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return list;

                EntityMapper<T, TableSchema> map = new EntityMapper<T, TableSchema>(includeTables);

                while (reader.Read())
                {
                    list.Add(map.ConvertObject(reader));
                }
                reader.Close();
            }

            return list;
        }

        ///// <summary>
        ///// 返回查询的结果集
        ///// </summary>
        ///// <returns></returns>
        //public List<TResult> ToList<TResult>(Func<T, TResult> exp)
        //{
        //    return this.ToList().Select(p => exp(p)).ToList();
        //}


        /// <summary>
        /// 自定义的返回结果
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        /// <remarks>
        /// 使用方法
        ///   var result =  [...].ToList(p => new { P1=p["name"], P2=p["id"] });
        ///   result[0].P1   result[0].P2
        /// </remarks>
        public List<TResult> ToList<TResult>(Func<IDataReader, TResult> exp)
        {
            List<TResult> retObjs = new List<TResult>();

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


        /// <summary>
        /// 用于返回自定义的数据结构
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        ///  <remarks>
        ///  var result = [...].ToList<Type1>();
        ///  result[0].P1   result[0].P2  返回的结果为Type1对象
        ///  </remarks>
        public List<TResult> ToList<TResult>() where TResult : class
        {
            List<TResult> retObjs = new List<TResult>();

            var drToObjectMapper = new DataReaderToObjectMapper<TResult>();
            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return retObjs;

                while (reader.Read())
                {
                    retObjs.Add(drToObjectMapper.ConvertObject(reader));
                }
                reader.Close();
            }
            return retObjs;
        }

        #endregion
    }
    /// <summary>
    /// 更新Section
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-10-18</para>
    /// </remarks>
    public sealed class UpdateSqlSection<T>
        where T : EntityBase
    {
        #region Private Members

        private readonly DataContext db;
        private readonly string tableName;
        private List<string> columnNames = new List<string>();
        private List<DbType> columnTypes = new List<DbType>();
        private List<object> columnValues = new List<object>();
        private DbTransaction tran;
        private WhereClip whereClip = new WhereClip();
        private TableSchema table;

        #endregion

        #region Constructors

        public UpdateSqlSection(DataContext db, EntityBase entity)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(entity != null, "entity could not be null.");

            this.db = db;


            foreach (var propName in entity.ChangedPropertys)
            {
                var v = entity.GetValue(propName);
                if (v.Column.IsPK)  //主键
                {
                    whereClip.And(v.Column == v.Value);
                }
                else
                {
                    columnNames.Add(v.Column.ShortName);
                    columnTypes.Add(v.Column.DbType);
                    columnValues.Add(v.Value);
                }

            }

            this.tableName = entity.GetTableName();
        }

        public UpdateSqlSection(DataContext db, TableSchema table)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(table != null, "table could not be null.");

            this.db = db;


            this.table = table;
            this.tableName = table.GetTableName();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 设置执行的事务
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        public UpdateSqlSection<T> SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }


        /// <summary>
        /// 自定义查询 设置某个字段值
        /// </summary>
        /// <param name="column">列</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public UpdateSqlSection<T> Set(QueryColumn column, object value)
        {
            if (column.IsPK) throw new Exception("自定义更新中，设定的字段不能是主键[" + column.Name + "]");
            columnNames.Add(column.ShortName);
            columnTypes.Add(column.DbType);
            columnValues.Add(value);
            return this;
        }

        /// <summary>
        /// 自定义查询 设置某个字段值
        /// </summary>
        /// <param name="column">列的表达式 p => p.Name</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UpdateSqlSection<T> Set(Func<TableSchema, QueryColumn> setExp, object value)
        {
            return this.Set(setExp(table), value);
        }


        /// <summary>
        /// 设置更新的条件
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public UpdateSqlSection<T> Where(WhereClip where)
        {
            whereClip.And(where);

            return this;
        }

        /// <summary>
        /// 设置更新的条件
        /// </summary>
        /// <param name="whereExp">lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <returns></returns>
        public UpdateSqlSection<T> Where(Func<TableSchema, WhereClip> whereExp)
        {
            return this.Where(whereExp(table));
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateUpdateCommand(tableName, whereClip, columnNames.ToArray(),
                columnTypes.ToArray(), columnValues.ToArray());
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
        }

        /// <summary>
        /// To the 数据库 command.
        /// </summary>
        /// <returns>DbCommand.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-18</para>
        /// </remarks>
        public DbCommand ToDbCommand()
        {
            return db.DbProvider.GetQueryFactory().CreateUpdateCommand(tableName, whereClip, columnNames.ToArray(),
                columnTypes.ToArray(), columnValues.ToArray());
        }

        /// <summary>
        /// To the 数据库 command text.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-18</para>
        /// </remarks>
        public string ToDbCommandText()
        {
            return ToDbCommandText(true);
        }

        /// <summary>
        /// If fillParameterValues == false, you must specify the parameter names you want to be in the returning sql.
        /// </summary>
        /// <param name="fillParameterValues"></param>
        /// <param name="parameterNames"></param>
        /// <returns></returns>
        public string ToDbCommandText(bool fillParameterValues, params string[] parameterNames)
        {
            if (fillParameterValues)
                return DataUtils.ToString(ToDbCommand());
            else
            {
                DbCommand cmd = ToDbCommand();
                string sql = cmd.CommandText;

                if (!string.IsNullOrEmpty(sql) && parameterNames != null)
                {
                    Check.Require(parameterNames.Length == cmd.Parameters.Count, "The Specified count of parameter names does not equal the count of parameter names in the query.");

                    System.Collections.IEnumerator en = cmd.Parameters.GetEnumerator();
                    int i = 0;
                    while (en.MoveNext())
                    {
                        Check.Require(parameterNames[i], "parameterNames[" + i + "]", Check.NotNullOrEmpty);

                        System.Data.Common.DbParameter p = (System.Data.Common.DbParameter)en.Current;
                        sql = sql.Replace(p.ParameterName, p.ParameterName[0] + parameterNames[i].TrimStart(p.ParameterName[0]));
                        ++i;
                    }
                }

                return sql;
            }
        }

        #endregion
    }

    /// <summary>
    /// 删除Section
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-10-17</para>
    /// </remarks>
    public sealed class DeleteSqlSection
    {
        #region Private Members

        /// <summary>
        /// The 数据库
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private readonly DataContext db;
        /// <summary>
        /// 表名
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private readonly string tableName;
        /// <summary>
        /// 事务
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private DbTransaction tran;
        /// <summary>
        /// The where clip
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private WhereClip whereClip = new WhereClip();
        /// <summary>
        /// The table
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        private TableSchema table;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteSqlSection"/> class.
        /// </summary>
        /// <param name="db">The 数据库.</param>
        /// <param name="table">The table.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public DeleteSqlSection(DataContext db, TableSchema table)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(table != null, "table could not be null.");

            this.db = db;
            this.table = table;
            this.tableName = table.GetTableName();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 设置事务
        /// </summary>
        /// <param name="tran">The tran.</param>
        /// <returns>DeleteSqlSection.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public DeleteSqlSection SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        /// <summary>
        /// 设置删除的条件
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns>DeleteSqlSection.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public DeleteSqlSection Where(WhereClip where)
        {
            whereClip.And(where);

            return this;
        }

        /// <summary>
        /// 设置删除的条件
        /// </summary>
        /// <param name="whereExp">The where exp.</param>
        /// <returns>DeleteSqlSection.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public DeleteSqlSection Where(Func<TableSchema, WhereClip> whereExp)
        {
            return this.Where(whereExp(table));
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public int Execute()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateDeleteCommand(tableName, whereClip);
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
        }

        /// <summary>
        /// To the 数据库 command.
        /// </summary>
        /// <returns>DbCommand.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public DbCommand ToDbCommand()
        {
            return db.DbProvider.GetQueryFactory().CreateDeleteCommand(tableName, whereClip);
        }

        /// <summary>
        /// To the 数据库 command text.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public string ToDbCommandText()
        {
            return ToDbCommandText(true);
        }

        /// <summary>
        /// If fillParameterValues == false, you must specify the parameter names you want to be in the returning sql.
        /// </summary>
        /// <param name="fillParameterValues">if set to <c>是</c> [fill parameter values].</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public string ToDbCommandText(bool fillParameterValues, params string[] parameterNames)
        {
            if (fillParameterValues)
                return DataUtils.ToString(ToDbCommand());
            else
            {
                DbCommand cmd = ToDbCommand();
                string sql = cmd.CommandText;

                if (!string.IsNullOrEmpty(sql) && parameterNames != null)
                {
                    Check.Require(parameterNames.Length == cmd.Parameters.Count, "The Specified count of parameter names does not equal the count of parameter names in the query.");

                    System.Collections.IEnumerator en = cmd.Parameters.GetEnumerator();
                    int i = 0;
                    while (en.MoveNext())
                    {
                        Check.Require(parameterNames[i], "parameterNames[" + i + "]", Check.NotNullOrEmpty);

                        System.Data.Common.DbParameter p = (System.Data.Common.DbParameter)en.Current;
                        sql = sql.Replace(p.ParameterName, p.ParameterName[0] + parameterNames[i].TrimStart(p.ParameterName[0]));
                        ++i;
                    }
                }

                return sql;
            }
        }

        #endregion
    }
}
