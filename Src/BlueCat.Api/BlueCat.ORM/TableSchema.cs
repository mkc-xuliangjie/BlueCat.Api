using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{
    /// <summary>
    /// 表基础架构
    /// </summary>
    /// <remarks>
    ///  	<para>创建：jiaj</para>
    ///  	<para>日期：2016-11-10</para>
    /// </remarks>
    public abstract class TableSchema
    {
        /// <summary>
        /// 名名称
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        private string tableName;
        /// <summary>
        /// 父表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        private TableSchema parentTable;
        /// <summary>
        /// 连接条件
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        internal WhereClip JoinWhere;
        /// <summary>
        /// 属性名称
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        private string propertyName;


        /// <summary>
        /// 表别名
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        private string tableAliasName;

        /// <summary>
        /// 表别名
        /// </summary>
        /// <value>The name of the table alias.</value>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        public string TableAliasName
        {
            get
            {
                if (string.IsNullOrEmpty(tableAliasName))
                {
                    tableAliasName = tableName;
                }
                return tableAliasName;
            }
            protected set
            {
                tableAliasName = value;
            }
        }


        /// <summary>
        /// 初始化构造函数 <see cref="TableSchema"/> .
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        public TableSchema(string tableName)
        {
            this.tableName = tableName;
        }


        /// <summary>
        /// 设置关系
        /// </summary>
        /// <param name="parentTable">父级表</param>
        /// <param name="joinWhere">连接条件</param>
        /// <param name="propertyName">属性名称</param>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        public void SetRelation(TableSchema parentTable, WhereClip joinWhere, string propertyName)
        {
            this.parentTable = parentTable;
            this.JoinWhere = joinWhere;
            this.propertyName = propertyName;
        }

        /// <summary>
        /// 列列表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        protected List<QueryColumn> columns = new List<QueryColumn>();
        /// <summary>
        ///外键表集合
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        private Dictionary<string, KeyValuePair<string, TableSchema>> foreignTables = new Dictionary<string, KeyValuePair<string, TableSchema>>();

        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="column">列</param>
        /// <exception cref="System.Exception">已包含重复的列</exception>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        protected void AddColumn(QueryColumn column)
        {
            column.Alias(column.Name);
            if (!columns.Any(c => c.Name == column.Name))
            {
                columns.Add(column);
            }
            else
            {
                throw new Exception("已包含重复的列");
            }
        }

        /// <summary>
        ///添加外键表
        /// </summary>
        /// <param name="foreignTable">外键表</param>
        /// <param name="entityPropertyName">关联的属性名称</param>
        /// <exception cref="System.Exception">已包含重复外键表</exception>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        protected void AddForeignTable(TableSchema foreignTable, string entityPropertyName)
        {
            var tableName = foreignTable.GetTableName();
            if (!foreignTables.ContainsKey(entityPropertyName))
            {
                foreignTables.Add(tableName, new KeyValuePair<string, TableSchema>(entityPropertyName, foreignTable));
            }
            else
            {
                throw new Exception("已包含重复外键表");
            }
        }

        /// <summary>
        /// 获取所有列表
        /// </summary>
        /// <returns>数据列表</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        internal List<QueryColumn> GetColumns()
        {
            return columns;
        }

        /// <summary>
        /// 获取合成的列集合
        /// </summary>
        /// <returns>列集合</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        internal string[] GetComplexColumns()
        {
            return columns.Select(c => c.Alias("[" + this.tableName + "_" + c.ShortName + "]").ToString()).ToArray();
        }

        /// <summary>
        /// 获取外键表列表
        /// </summary>
        /// <returns>Dictionary&lt;System.String, KeyValuePair&lt;System.String, TableSchema&gt;&gt;.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        internal Dictionary<string, KeyValuePair<string, TableSchema>> GetForeignTables()
        {
            return foreignTables;
        }


        /// <summary>
        /// 获取所有的列
        /// </summary>
        /// <value>获取所有的列.</value>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        public QueryColumn All
        {
            get { return new QueryColumn("[" + this.tableName + "].*", System.Data.DbType.Int32); }
        }

        /// <summary>
        /// 数量
        /// </summary>
        /// <value>The count.</value>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        public QueryColumn Count
        {
            get { return new QueryColumn("Count(*)", System.Data.DbType.Int32); }
        }


        /// <summary>
        ///根据列名获取列信息
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>QueryColumn.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        internal QueryColumn GetColumn(string name)
        {
            return columns.FirstOrDefault(c => c.Name == name);
        }

        /// <summary>
        /// 主键列
        /// </summary>
        /// <value>The pk column.</value>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        internal QueryColumn PKColumn
        {
            get
            {
                if (!isGetPKColumn)
                {
                    pkColumn = columns.FirstOrDefault(c => c.IsPK == true);
                }
                return pkColumn;
            }
            /// <summary>
            /// The is get pk column
            /// </summary>
            /// <remarks>
            ///  	<para>创建：jiaj</para>
            ///  	<para>日期：2016-11-10</para>
            /// </remarks>
        } private bool isGetPKColumn = false;


        /// <summary>
        /// Gets a value indicating whether this instance is foreign table.
        /// </summary>
        /// <value><c>是</c> if this instance is foreign table; otherwise, <c>否</c>.</value>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        internal bool IsForeignTable { get { return parentTable != null; } }

        /// <summary>
        /// The pk column
        /// </summary>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        private QueryColumn pkColumn;

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        public string GetTableName()
        {
            return tableName;
        }

        /// <summary>
        /// Gets the name of the table alias.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        protected string GetTableAliasName()
        {
            return string.IsNullOrEmpty(tableAliasName) ? tableName : tableAliasName;
        }



        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        internal string GetPropertyName()
        {
            if (IsForeignTable)
            {
                var pn = parentTable.GetPropertyName();
                if (string.IsNullOrEmpty(pn))
                {
                    return propertyName;
                }
                else
                {
                    return pn + "." + this.propertyName;
                }

            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Onlies the specified columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>ExpressionClip[].</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        [Obsolete("建议使用Field代替Only")]
        public ExpressionClip[] Only(params ExpressionClip[] columns)
        {
            return columns;
        }

        /// <summary>
        /// Fields the specified columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>ExpressionClip[].</returns>
        /// <remarks>
        ///  	<para>创建：jiaj</para>
        ///  	<para>日期：2016-11-10</para>
        /// </remarks>
        public ExpressionClip[] Field(params ExpressionClip[] columns)
        {
            return columns;
        }

    }

}
