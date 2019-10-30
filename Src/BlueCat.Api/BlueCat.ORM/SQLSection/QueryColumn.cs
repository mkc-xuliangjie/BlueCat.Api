using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{
    /// <summary>
    /// 查询列
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2018/3/22</para>
    /// </remarks>
    public sealed class QueryColumn : ExpressionClip
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        internal string Name
        {
            get
            {
                return this.ToString().Replace("[", string.Empty).Replace("]", string.Empty);
            }
        }

        /// <summary>
        /// Gets the short name.
        /// </summary>
        /// <value>The short name.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        internal string ShortName
        {
            get
            {
                return Name.IndexOf('.') > 0 ? Name.Split('.')[1] : Name;
            }
        }

        /// <summary>
        /// Gets the name of the entity property.
        /// </summary>
        /// <value>The name of the entity property.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        internal string EntityPropertyName
        {
            get
            {
                if (string.IsNullOrEmpty(entityPropertyName))
                {
                    return ShortName;
                }
                return entityPropertyName;
            }
        }
        /// <summary>
        /// The entity property name
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        private string entityPropertyName;


        /// <summary>
        /// Gets or sets a value indicating whether this instance is pk.
        /// </summary>
        /// <value><c>true</c> if this instance is pk; otherwise, <c>false</c>.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        internal bool IsPK { get; set; }

        /// <summary>
        /// 是否为自增长
        /// </summary>
        /// <value><c>true</c> if this instance is automatic increment; otherwise, <c>false</c>.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        internal bool IsAutoIncrement { get; set; }

        /// <summary>
        /// Gets the desc.
        /// </summary>
        /// <value>The desc.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public OrderByClip Desc
        {
            get
            {
                return new OrderByClip(this, true);
            }
        }

        /// <summary>
        /// Gets the asc.
        /// </summary>
        /// <value>The asc.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public OrderByClip Asc
        {
            get
            {
                return new OrderByClip(this, false);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryColumn"/> class.
        /// </summary>
        /// <param name="dbColName">Name of the database col.</param>
        /// <param name="type">The type.</param>
        /// <param name="isAutoIncrement">字段是否为自增长</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public QueryColumn(string dbColName, DbType type, bool isAutoIncrement = false)
            : this(dbColName, type, "", false, isAutoIncrement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryColumn"/> class.
        /// </summary>
        /// <param name="dbColName">Name of the database col.</param>
        /// <param name="type">The type.</param>
        /// <param name="entityPropertyName">Name of the entity property.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public QueryColumn(string dbColName, DbType type, string entityPropertyName)
            : this(dbColName, type, entityPropertyName, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryColumn"/> class.
        /// </summary>
        /// <param name="dbColName">Name of the database col.</param>
        /// <param name="type">The type.</param>
        /// <param name="entityPropertyName">Name of the entity property.</param>
        /// <param name="isPk">if set to <c>true</c> [is pk].</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public QueryColumn(string dbColName, DbType type, string entityPropertyName, bool isPk) : this(dbColName, type, entityPropertyName, isPk, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryColumn"/> class.
        /// </summary>
        /// <param name="dbColName">Name of the database col.</param>
        /// <param name="type">The type.</param>
        /// <param name="entityPropertyName">Name of the entity property.</param>
        /// <param name="isPk">if set to <c>true</c> [is pk].</param>
        /// <param name="isAutoIncrement">是否为自增长属性</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2018/3/22</para>
        /// </remarks>
        public QueryColumn(string dbColName, DbType type, string entityPropertyName, bool isPk, bool isAutoIncrement)
        {
            Check.Require(!string.IsNullOrEmpty(dbColName), "dbColName could not be null or empty.");

            SqlQueryUtils.AppendColumnName(this.sql, dbColName);
            this.entityPropertyName = entityPropertyName;
            this.DbType = type;
            this.IsPK = isPk;
            this.IsAutoIncrement = isAutoIncrement;
        }
    }
}
