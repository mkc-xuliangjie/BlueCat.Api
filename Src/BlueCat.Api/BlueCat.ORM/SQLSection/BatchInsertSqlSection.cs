
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{
    /// <summary>
    /// 批量插入
    /// </summary>
    /// <remarks><para>创建：Teddy</para>
    /// <para>日期：2016-11-14</para></remarks>
    public sealed class BatchInsertSqlSection : IInsertSqlSection
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
        private string[] columnNames;
        /// <summary>
        /// The column types
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private DbType[] columnTypes;
        /// <summary>
        /// The column values
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private object[][] columnValues = null;
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
        /// Initializes a new instance of the <see cref="BatchInsertSqlSection"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="entitys">The entitys.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public BatchInsertSqlSection(DataContext db, TableSchema schema, EntityBase[] entitys)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(entity != null, "entity could not be null.");

            this.db = db;

            var cols = schema.GetColumns().Where(p=>!p.IsAutoIncrement).ToList();
            tableName = schema.GetTableName();
            columnNames = cols.Select(p => "[" + p.ShortName + "]").ToArray();
            var entityPropertyNames = cols.Select(p => p.EntityPropertyName).ToArray();
            columnTypes = cols.Select(p => p.DbType).ToArray();
            columnValues = new object[entitys.Length][];
            for (int i = 0; i < entitys.Length; i++)
            {
                var row = new object[cols.Count];
                for (int j = 0; j < cols.Count; j++)
                {
                    var value = entitys[i].GetValue(entityPropertyNames[j]);
                    row[j] = value.Value;
                }
                columnValues[i] = row;
            }


            this.tableName = schema.GetTableName();
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
            return tran == null ? db.ExecuteBatchInsert(tableName, columnNames, columnTypes, columnValues, columnValues.Count())
                                : db.ExecuteBatchInsert(tableName, columnNames, columnTypes, columnValues, columnValues.Count(), tran);
        }
        

        #endregion
    }

}
