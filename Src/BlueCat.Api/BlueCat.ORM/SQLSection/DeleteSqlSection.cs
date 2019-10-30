using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{




    /// <summary>
    /// 删除 SqlSection
    /// </summary>
    /// <typeparam name="TSchema">The type of the t schema.</typeparam>
    /// <remarks><para>创建：Teddy</para>
    /// <para>日期：2018/3/22</para></remarks>
    public sealed class DeleteSqlSection<TSchema>
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
        private WhereClip whereClip = new WhereClip();
        /// <summary>
        /// The table
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private TSchema table;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteSqlSection{TSchema}"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="table">The table.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public DeleteSqlSection(DataContext db, TSchema table)
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
        /// <returns>DeleteSqlSection&lt;TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public DeleteSqlSection<TSchema> SetTransaction(DbTransaction tran)
        {
            this.tran = tran;

            return this;
        }

        /// <summary>
        /// 设置删除的条件
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns>DeleteSqlSection&lt;TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public DeleteSqlSection<TSchema> Where(WhereClip where)
        {
            if (!WhereClip.IsNullOrEmpty(where))
            {
                whereClip.And(where);
            }
            return this;
        }

        /// <summary>
        /// 设置删除的条件
        /// </summary>
        /// <param name="whereExp">The where exp.</param>
        /// <returns>DeleteSqlSection&lt;TSchema&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public DeleteSqlSection<TSchema> Where(Func<TSchema, WhereClip> whereExp)
        {
            return this.Where(whereExp(table));
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public int Execute()
        {
            DbCommand cmd = db.DbProvider.GetQueryFactory().CreateDeleteCommand(tableName, whereClip);
            return tran == null ? db.ExecuteNonQuery(cmd) : db.ExecuteNonQuery(cmd, tran);
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
            return db.DbProvider.GetQueryFactory().CreateDeleteCommand(tableName, whereClip);
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
    }

}
