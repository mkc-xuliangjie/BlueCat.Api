using System;
using System.Collections.Generic;
using System.Text;


namespace BlueCat.ORM
{

    /// <summary>
    /// 表结构
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-10-17</para>
    /// </remarks>
    public struct TableStruct
    {
        /// <summary>
        /// 表或视图名称
        /// </summary>
        /// <value>The name of the table or view.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public string TableOrViewName { get; set; }

        /// <summary>
        /// 表或视图别名
        /// </summary>
        /// <value>The name of the table or view alias.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public string TableOrViewAliasName { get; set; }

        /// <summary>
        /// 0 INNER 
        /// 1 LEFT OUTER
        /// 2 RITHG OUTER
        /// </summary>
        /// <value>The type of the join.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        public JoinType JoinType { get; set; }
    }

    /// <summary>
    /// 连接类型
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-10-17</para>
    /// </remarks>
    public enum JoinType : byte
    {
        /// <summary>
        /// 内连接
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        INNER = 0,
        /// <summary>
        /// 左外连接
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        LEFTOUTER,
        /// <summary>
        /// 右外连接
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        RIGHTOUTER
    }


    /// <summary>
    /// From 表达式
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-10-17</para>
    /// </remarks>
    public class FromClip
    {
        #region Protected Members

        /// <summary>
        /// Join 
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        internal readonly Dictionary<TableStruct, KeyValuePair<string, WhereClip>> joins = new Dictionary<TableStruct, KeyValuePair<string, WhereClip>>();

        /// <summary>
        /// 主表名
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        protected readonly TableSchema table;
        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the table or view.
        /// </summary>
        /// <value>The name of the table or view.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public string TableOrViewName
        {
            get
            {
                return table.GetTableName();
            }
        }


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FromClip"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public FromClip(TableSchema table)
        {
            this.table = table;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Joins the specified forgein table.
        /// </summary>
        /// <param name="forgeinTable">The forgein table.</param>
        /// <returns>FromClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public FromClip Join(TableSchema forgeinTable)
        {
            return this.Join(forgeinTable, forgeinTable.JoinWhere);
        }
        /// <summary>
        /// Joins the specified forgein table.
        /// </summary>
        /// <param name="forgeinTable">The forgein table.</param>
        /// <param name="onWhere">The on where.</param>
        /// <param name="joinType">Type of the join.</param>
        /// <returns>FromClip.</returns>
        /// <exception cref="NameDuplicatedException">In joins list: tableName -  + tableName</exception>
        /// <remarks><para>创建：Teddy</para>
        /// <para>日期：2016-9-7</para></remarks>
        public FromClip Join(TableSchema forgeinTable, WhereClip onWhere, JoinType joinType = JoinType.INNER)
        {
            var tableName = forgeinTable.GetTableName();
            var joinStruct = new TableStruct { TableOrViewAliasName = forgeinTable.TableAliasName, TableOrViewName = tableName, JoinType = joinType };

            if (joins.ContainsKey(joinStruct))
            {
                throw new NameDuplicatedException("In joins list: tableName - " + tableName);
            }

            joins.Add(joinStruct, new KeyValuePair<string, WhereClip>(tableName, onWhere));
            return this;
        }

        /// <summary>
        /// Join是否已经存在
        /// </summary>
        /// <param name="forgeinTable">关联表</param>
        /// <param name="joinType">Join类型</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        internal bool ExitJoin(TableSchema forgeinTable, JoinType joinType = JoinType.INNER)
        {
            var tableName = forgeinTable.GetTableName();
            var joinStruct = new TableStruct { TableOrViewAliasName = forgeinTable.TableAliasName, TableOrViewName = tableName, JoinType = joinType };

            return joins.ContainsKey(joinStruct);
        }





        /// <summary>
        /// 获取Join连接字符串
        /// </summary>
        /// <param name="joinType">join类型</param>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public string GetJoinString(JoinType joinType)
        {
            var joinString = "";
            switch (joinType)
            {
                case JoinType.LEFTOUTER:
                    joinString = " LEFT OUTER JOIN ";
                    break;
                case JoinType.RIGHTOUTER:
                    joinString = " RIGHT OUTER JOIN ";
                    break;
                default:
                    joinString = " INNER JOIN ";
                    break;
            }
            return joinString;


        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-10-17</para>
        /// </remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            var tableOrViewName = table.GetTableName();
            if (!tableOrViewName.Contains("["))
            {
                sb.Append('[');
                sb.Append(tableOrViewName);
                sb.Append(']');
            }
            else
                sb.Append(tableOrViewName);

            foreach (TableStruct joinStruct in joins.Keys)
            {

                if (sb.ToString().Contains("JOIN"))
                {
                    sb = new StringBuilder('(' + sb.ToString() + ')');
                }

                KeyValuePair<string, WhereClip> keyWhere = joins[joinStruct];

                sb.Append(GetJoinString(joinStruct.JoinType));

                if (joinStruct.TableOrViewName != keyWhere.Key)
                {
                    if (!joinStruct.TableOrViewName.Contains("["))
                    {
                        sb.Append('[');
                        sb.Append(joinStruct.TableOrViewName);
                        sb.Append(']');

                    }
                    else
                        sb.Append(joinStruct.TableOrViewName);

                }

                sb.Append(' ');
                if (!keyWhere.Key.Contains("["))
                {
                    sb.Append('[');
                    sb.Append(keyWhere.Key);
                    sb.Append(']');
                }
                else
                    sb.Append(keyWhere.Key);

                if (joinStruct.TableOrViewName != joinStruct.TableOrViewAliasName)
                {
                    sb.AppendFormat("AS {0} ", joinStruct.TableOrViewAliasName);
                }

                sb.Append(" ON ");
                sb.Append(keyWhere.Value.ToString());
            }

            return sb.ToString();
        }

        #endregion
    }
}
