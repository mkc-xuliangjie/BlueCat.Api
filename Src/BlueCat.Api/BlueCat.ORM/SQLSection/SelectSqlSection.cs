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
    /// 查询SQLSection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSchema">The type of the t schema.</typeparam>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2018/3/22</para>
    /// </remarks>
    public sealed class SelectSqlSection<T, TSchema>
        where T : EntityBase
        where TSchema : TableSchema
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
        /// The table name
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private readonly string tableName;
        /// <summary>
        /// The column names
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private string[] columnNames = new string[] { "*" };
        /// <summary>
        /// The tran
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private DbTransaction tran;
        /// <summary>
        /// The where clip
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private WhereClip whereClip;
        /// <summary>
        /// The top item count
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private int topItemCount = int.MaxValue;
        /// <summary>
        /// The skip item count
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private int skipItemCount = 0;
        /// <summary>
        /// The identy column name
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private string identyColumnName;
        /// <summary>
        /// The identy column is number
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private bool identyColumnIsNumber = false;
        /// <summary>
        /// The table
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private TSchema table;
        /// <summary>
        /// The include tables
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private Dictionary<string, TableSchema> includeTables = new Dictionary<string, TableSchema>();

        /// <summary>
        /// Prepares the command.
        /// </summary>
        /// <returns>DbCommand.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
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

                //Console.WriteLine("执行脚本：{0}", cmd.CommandText);
            }
            return cmd;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectSqlSection{T, TSchema}"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection(DataContext db, TSchema table, params ExpressionClip[] columns)
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectSqlSection{T, TSchema}"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="table">The table.</param>
        /// <param name="whereClip">The where clip.</param>
        /// <param name="columns">The columns.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection(DataContext db, TSchema table, WhereClip whereClip, params ExpressionClip[] columns)
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
        /// <param name="tran">The tran.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        /// <summary>
        /// Wheres the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> Where(WhereClip where)
        {
            if (!WhereClip.IsNullOrEmpty(where))
            {
                whereClip.And(where);
            }
            return this;
        }

        /// <summary>
        /// Wheres the specified where exp.
        /// </summary>
        /// <param name="whereExp">The where exp.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> Where(Func<TSchema, WhereClip> whereExp)
        {
            return this.Where(whereExp(table));
        }

        /// <summary>
        /// Wheres the specified f table.
        /// </summary>
        /// <typeparam name="TForgeinTable">The type of the t forgein table.</typeparam>
        /// <param name="fTable">The f table.</param>
        /// <param name="whereExp">The where exp.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> Where<TForgeinTable>(TForgeinTable fTable, Func<TForgeinTable, WhereClip> whereExp) where TForgeinTable : TableSchema, new()
        {
            return this.Where(whereExp(fTable));
        }


        /// <summary>
        /// 数据库中符合当前记录的数
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        [Obsolete("建议使用GetTotalForPaging()，此方法命名容易产生歧义")]
        public int GetCount()
        {
            return GetTotalForPaging();
        }

        /// <summary>
        /// Gets the total for paging.
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public int GetTotalForPaging()
        {
            var where = this.whereClip.Clone() as WhereClip;
            where.OrderBy = string.Empty;

            var result = new SelectSqlSection<T, TSchema>(db, table, where, table.Count).ToScalar();
            return result is int ? (int)result : int.Parse(result.ToString());
        }


        /// <summary>
        /// 设置排序的字段
        /// </summary>
        /// <param name="orderBys">排序的字段</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> OrderBy(params OrderByClip[] orderBys)
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

        /// <summary>
        /// 设置排序的字段
        /// </summary>
        /// <param name="orderExp">排序字段：lambda表达式 如 ： p=&gt; p.Name.Asc 或者 p=&gt;p.Name.Desc</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> OrderBy(Func<TSchema, OrderByClip> orderExp)
        {
            orderByClips.Add(orderExp(table));
            return this.OrderBy(orderByClips.ToArray());
        }
        /// <summary>
        /// The order by clips
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private List<OrderByClip> orderByClips = new List<OrderByClip>();

        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> GroupBy(params QueryColumn[] columns)
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

        /// <summary>
        /// Havings the specified where clip.
        /// </summary>
        /// <param name="whereClip">The where clip.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> Having(WhereClip whereClip)
        {
            this.whereClip.SetHaving(whereClip);
            return this;
        }

        /// <summary>
        /// Havings the specified where exp.
        /// </summary>
        /// <param name="whereExp">The where exp.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> Having(Func<TSchema, WhereClip> whereExp)
        {
            this.whereClip.SetHaving(whereExp(table));
            return this;
        }


        /// <summary>
        /// 设置分组的字段
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> GroupBy(Func<TSchema, QueryColumn> exp)
        {
            return this.GroupBy(exp(table));
        }

        /// <summary>
        /// 设置分组的字段
        /// </summary>
        /// <param name="exp">排序字段：lambda表达式 如 ： p=&gt; p.Name.Asc 或者 p=&gt;p.Name.Desc</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> GroupBy(Func<TSchema, ExpressionClip[]> exp)
        {
            var cols = exp(table);
            var groupCols = cols.Select(p => p as QueryColumn);
            return this.GroupBy(groupCols.ToArray());
        }

        /// <summary>
        /// 设置获取数据的范围
        /// </summary>
        /// <param name="topItemCount">The top item count.</param>
        /// <param name="skipItemCount">The skip item count.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> SetSelectRange(int topItemCount, int skipItemCount)
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
        /// <param name="topCount">获取数据的条数</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> Top(int topCount)
        {
            Check.Require(topCount > 0, "topCount MUST > 0");
            this.topItemCount = topCount;

            return this;
        }

        /// <summary>
        /// 设置获取数据的条数 = 1
        /// </summary>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public SelectSqlSection<T, TSchema> Top()
        {
            return this.Top(1);
        }


        /// <summary>
        /// 包含外键数据
        /// </summary>
        /// <param name="foreignTable">The foreign table.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <exception cref="Exception">这不是一个连接表</exception>
        /// <exception cref="System.Exception">这不是一个连接表</exception>
        /// <remarks><para>创建：Teddy</para>
        /// <para>日期：2016-10-18</para></remarks>
        public SelectSqlSection<T, TSchema> Include(TableSchema foreignTable)
        {

            if (!foreignTable.IsForeignTable) throw new Exception("这不是一个连接表");

            var cols = new List<string>();
            cols.AddRange(columnNames);
            cols.AddRange(foreignTable.GetComplexColumns());
            this.columnNames = cols.ToArray();

            includeTables.Add(foreignTable.GetPropertyName(), foreignTable);

            return this.Join(foreignTable, foreignTable.JoinWhere);
        }


        /// <summary>
        /// 包含外键数据
        /// </summary>
        /// <param name="exp">管理的表达式  p =&gt; p.UserType [UserType为外键表]</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks><para>创建：Teddy</para>
        /// <para>日期：2016-10-18</para></remarks>
        public SelectSqlSection<T, TSchema> Include(Func<TSchema, TableSchema> exp)
        {
            return this.Include(exp(table));
        }


        /// <summary>
        /// 连接设置
        /// </summary>
        /// <param name="joinTable">主表跟当前表进行连接</param>
        /// <param name="joinOnWhere">连接条件</param>
        /// <param name="joinType">连接类型，支持内连接、左外连接、右外连接</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks><para>创建：Teddy</para>
        /// <para>日期：2016-10-18</para></remarks>
        public SelectSqlSection<T, TSchema> Join(TableSchema joinTable, WhereClip joinOnWhere, JoinType joinType = JoinType.INNER)
        {
            Check.Require(joinTable != null, "joinTable could not be null.");
            Check.Require(!WhereClip.IsNullOrEmpty(joinOnWhere), "joinOnWhere could not be null or empty.");

            whereClip.AddOtherPararms(joinOnWhere.Parameters);

            this.whereClip.From.Join(joinTable, joinOnWhere, joinType);

            return this;
        }

        /// <summary>
        /// 连接设置
        /// </summary>
        /// <typeparam name="FTable">连接表</typeparam>
        /// <param name="fTable">连接表</param>
        /// <param name="joinOnWhereExp">连接条件</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks><para>创建：Teddy</para>
        /// <para>日期：2016-10-18</para></remarks>
        public SelectSqlSection<T, TSchema> Join<FTable>(FTable fTable, Func<TSchema, FTable, WhereClip> joinOnWhereExp) where FTable : TableSchema, new()
        {
            return this.Join(fTable, joinOnWhereExp(this.table, fTable), JoinType.INNER);
        }

        /// <summary>
        /// 设置连接
        /// </summary>
        /// <typeparam name="FTable">连接表类型</typeparam>
        /// <param name="fTable">连接表</param>
        /// <param name="joinOnWhereExp">连接条件</param>
        /// <param name="joinType">Type of the join.</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks><para>创建：Teddy</para>
        /// <para>日期：2016-10-18</para></remarks>
        public SelectSqlSection<T, TSchema> Join<FTable>(FTable fTable, Func<TSchema, FTable, WhereClip> joinOnWhereExp, JoinType joinType) where FTable : TableSchema, new()
        {
            return this.Join(fTable, joinOnWhereExp(this.table, fTable), joinType);
        }

        /// <summary>
        /// 设置连接
        /// </summary>
        /// <typeparam name="FTable">连接表1</typeparam>
        /// <typeparam name="FTable1">连接表二</typeparam>
        /// <param name="fTable">连接表1</param>
        /// <param name="fTable1">连接表2</param>
        /// <param name="joinOnWhereExp">连接条件</param>
        /// <param name="joinType">连接类型</param>
        /// <returns>SelectSqlSection&lt;T, TSchema&gt;.</returns>
        /// <remarks><para>创建：Teddy</para>
        /// <para>日期：2016-10-18</para></remarks>
        public SelectSqlSection<T, TSchema> Join<FTable, FTable1>(FTable fTable, FTable1 fTable1, Func<FTable, FTable1, WhereClip> joinOnWhereExp, JoinType joinType = JoinType.INNER)
            where FTable : TableSchema, new()
            where FTable1 : TableSchema, new()
        {
            if (!this.whereClip.From.ExitJoin(fTable))
            {
                return this.Join(fTable, joinOnWhereExp(fTable, fTable1), joinType);
            }
            else
            {
                return this.Join(fTable1, joinOnWhereExp(fTable, fTable1), joinType);
            }
        }



        /// <summary>
        /// 返回单独结果
        /// </summary>
        /// <returns>System.Object.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public object ToScalar()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteScalar(cmd) : db.ExecuteScalar(cmd, tran);
        }

        /// <summary>
        /// 返回一个DataReader结果集
        /// </summary>
        /// <returns>IDataReader.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public IDataReader ToDataReader()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteReader(cmd) : db.ExecuteReader(cmd, tran);
        }

        /// <summary>
        /// 返回一个DataSet结果集
        /// </summary>
        /// <returns>DataSet.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public DataSet ToDataSet()
        {
            DbCommand cmd = PrepareCommand();
            return tran == null ? db.ExecuteDataSet(cmd) : db.ExecuteDataSet(cmd, tran);
        }

        /// <summary>
        /// 返回单独实体
        /// </summary>
        /// <returns>T.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public T ToSingleObject()
        {
            T retObj = default(T);

            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return retObj;

                EntityMapper<T, TSchema> map = new EntityMapper<T, TSchema>(includeTables);
                if (reader.Read())
                    retObj = map.ConvertObject(reader);
                reader.Close();
            }

            return retObj;
        }


        /// <summary>
        /// 返回单独实体
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="exp">The exp.</param>
        /// <returns>TResult.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
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
        /// <returns>List&lt;T&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public List<T> ToList()
        {
            var list = new List<T>();
            using (IDataReader reader = this.ToDataReader())
            {
                if (reader == null)
                    return list;

                EntityMapper<T, TSchema> map = new EntityMapper<T, TSchema>(includeTables);

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
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="exp">表达式</param>
        /// <returns>List&lt;TResult&gt;.</returns>
        /// <remarks>使用方法
        /// var result =  [...].ToList(p =&gt; new { P1=p["name"], P2=p["id"] });
        /// result[0].P1   result[0].P2</remarks>
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
        /// To the list.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <returns>List&lt;TResult&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public override string ToString()
        {

            DbCommand cmd = PrepareCommand();
            return cmd.CommandText;
        }


    }

}
