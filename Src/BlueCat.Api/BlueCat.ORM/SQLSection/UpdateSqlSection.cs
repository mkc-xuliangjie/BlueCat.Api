using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{
    public sealed class UpdateSqlSection<T, TSchema>
        where T : EntityBase
        where TSchema : TableSchema
    {
        #region Private Members

        private readonly DataContext db;
        private readonly string tableName;
        private List<string> columnNames = new List<string>();
        private List<DbType> columnTypes = new List<DbType>();
        private List<object> columnValues = new List<object>();
        private DbTransaction tran;
        private WhereClip whereClip = new WhereClip();
        private TSchema table;

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
                    if (v.Column.DbType == DbType.Time)
                    {
                        columnValues.Add(v.Value.ToString());
                    }
                    else
                    {
                        columnValues.Add(v.Value);
                    }
                }

            }

            this.tableName = entity.GetTableName();
        }

        public UpdateSqlSection(DataContext db, TSchema table)
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
        public UpdateSqlSection<T, TSchema> SetTransaction(DbTransaction tran)
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
        public UpdateSqlSection<T, TSchema> Set(QueryColumn column, object value)
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
        public UpdateSqlSection<T, TSchema> Set(Func<TSchema, QueryColumn> setExp, object value)
        {
            return this.Set(setExp(table), value);
        }


        /// <summary>
        /// 设置更新的条件
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public UpdateSqlSection<T, TSchema> Where(WhereClip where)
        {
            whereClip.And(where);

            return this;
        }

        /// <summary>
        /// 设置更新的条件
        /// </summary>
        /// <param name="whereExp">lambda表达式 如 ： p=> p.Name =="2"  或者 p=> p.Name=="2" && p.ID ==1</param>
        /// <returns></returns>
        public UpdateSqlSection<T, TSchema> Where(Func<TSchema, WhereClip> whereExp)
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

        public DbCommand ToDbCommand()
        {
            return db.DbProvider.GetQueryFactory().CreateUpdateCommand(tableName, whereClip, columnNames.ToArray(),
                columnTypes.ToArray(), columnValues.ToArray());
        }

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

}
