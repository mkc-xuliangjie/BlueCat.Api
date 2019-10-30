using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{
    /// <summary>
    /// 掺入 SQLSection
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2018/3/22</para>
    /// </remarks>
    public class InsertSqlSection : IInsertSqlSection
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
        private List<string> columnNames = new List<string>();
        /// <summary>
        /// The column types
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private List<DbType> columnTypes = new List<DbType>();
        /// <summary>
        /// The column values
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private List<object> columnValues = new List<object>();
        /// <summary>
        /// The tran
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private DbTransaction tran;

        #endregion

        #region Constructors


        /// <summary>
        /// The entity
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private EntityBase entity;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertSqlSection"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="entity">The entity.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public InsertSqlSection(DataContext db, EntityBase entity)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(entity != null, "entity could not be null.");

            this.db = db;
            this.entity = entity;
            foreach (var propName in entity.ChangedPropertys)
            {
                var v = entity.GetValue(propName);
                columnNames.Add("[" + v.Column.ShortName + "]");
                columnTypes.Add(v.Column.DbType);
                if (v.Column.DbType == DbType.Time)
                {
                    columnValues.Add(v.Value.ToString());
                }
                else
                {
                    columnValues.Add(v.Value);
                }

            }
            this.tableName = entity.GetTableName();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 设置执行的事务
        /// </summary>
        /// <param name="tran">事务</param>
        /// <returns>返回插入部分</returns>
        /// <remarks><para>创建：Teddy</para>
        /// <para>日期：2016-9-7</para></remarks>
        public IInsertSqlSection SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        /// <summary>
        /// 立即执行操作
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <remarks><para>创建：Teddy</para>
        /// <para>日期：2016-9-7</para></remarks>
        public int Execute()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateInsertCommand(tableName, entity.ChangedPropertys.ToArray(),
                columnTypes.ToArray(), columnValues.ToArray());
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
        }


        /// <summary>
        /// Executes the return automatic increment identifier.
        /// </summary>
        /// <param name="autoIncrementColumn">The automatic increment column.</param>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public int ExecuteReturnAutoIncrementID(QueryColumn autoIncrementColumn)
        {
            Check.Require(!QueryColumn.IsNullOrEmpty(autoIncrementColumn), "autoIncrementColumn could not be null or empty.");

            string filteredAutoColumn = autoIncrementColumn.Name.IndexOf('.') > 0 ? autoIncrementColumn.Name.Split('.')[1] : autoIncrementColumn.Name;

            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateInsertCommand(tableName, columnNames.ToArray(),
                columnTypes.ToArray(), columnValues.ToArray());
            return tran == null ? db.ExecuteInsertReturnAutoIncrementID(cmd, tableName, filteredAutoColumn)
                : db.ExecuteInsertReturnAutoIncrementID(cmd, tran, tableName,
                filteredAutoColumn);
        }

        /// <summary>
        /// To the database command.
        /// </summary>
        /// <returns>DbCommand.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public DbCommand ToDbCommand()
        {
            return db.DbProvider.GetQueryFactory().CreateInsertCommand(tableName, columnNames.ToArray(),
                columnTypes.ToArray(), columnValues.ToArray());
        }

        /// <summary>
        /// To the database command text.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public string ToDbCommandText()
        {
            return ToDbCommandText(true);
        }

        /// <summary>
        /// If fillParameterValues == false, you must specify the parameter names you want to be in the returning sql.
        /// </summary>
        /// <param name="fillParameterValues">if set to <c>true</c> [fill parameter values].</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
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
            return ToDbCommandText();
        }
        
    }

}
