using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;


namespace BlueCat.ORM
{
    /// <summary>
    /// 表达式基类
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-9-8</para>
    /// </remarks>
    public abstract class ExpressionBase : IExpression
    {
        #region Protected Members

        /// <summary>
        /// The SQL
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        protected StringBuilder sql = new StringBuilder();
        /// <summary>
        /// 参数列表
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        protected readonly Dictionary<string, KeyValuePair<DbType, object>> parameters = new Dictionary<string, KeyValuePair<DbType, object>>();

        /// <summary>
        /// Makes the unique 参数 name without prefix token.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        internal protected static string MakeUniqueParamNameWithoutPrefixToken()
        {
            return CommonUtils.MakeUniqueKey(15, "p");
        }

        #endregion

        #region IExpression Members

        /// <summary>
        /// 脚本
        /// </summary>
        /// <value>The SQL.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public string Sql
        {
            get
            {
                return sql.ToString();
            }
            set
            {
                sql = new StringBuilder(value);
            }
        }

        /// <summary>
        /// 参数列表
        /// </summary>
        /// <value>The parameters.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public Dictionary<string, KeyValuePair<DbType, object>> Parameters
        {
            get
            {

                return parameters;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public override string ToString()
        {
            return sql.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Class ExpressionClip
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-9-8</para>
    /// </remarks>
    public class ExpressionClip : ExpressionBase, ICloneable
    {
        #region Protected Members

        /// <summary>
        /// 数据类型
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        protected DbType dbType;

        #endregion

        #region Properties

        /// <summary>
        /// 数据类型
        /// </summary>
        /// <value>The type of the 数据库.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public DbType DbType
        {
            get
            {
                return dbType;
            }
            set
            {
                dbType = value;
            }
        }

        #endregion

        #region Constructors & factory methods

        /// <summary>
        /// 初始化构造函数<see cref="ExpressionClip"/> 
        /// </summary>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip()
        {
        }

        /// <summary>
        /// 初始化列表达式
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="type">The type.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        protected void InitColumnExpression(string columnName, DbType type)
        {
            if (this.sql.Length > 0)
            {
                this.sql = new StringBuilder();
            }
            SqlQueryUtils.AppendColumnName(this.sql, columnName);
            this.DbType = type;
        }

        /// <summary>
        /// Initializes a new Column Expression instance of the <see cref="ExpressionClip" /> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="type">The type.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip(string columnName, DbType type)
        {
            Check.Require(columnName != null, "columnName could not be null!");

            InitColumnExpression(columnName, type);
        }

        /// <summary>
        /// Initializes a new Parameter Expression instance of the <see cref="ExpressionClip" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip(DbType type, object value)
        {
            string paramName = MakeUniqueParamNameWithoutPrefixToken();
            this.sql.Append('@');
            this.sql.Append(paramName.TrimStart('@'));
            this.parameters.Add(paramName, new KeyValuePair<DbType, object>(type, value));
            this.dbType = type;
        }


        internal void AddOtherPararms(Dictionary<string, KeyValuePair<DbType, object>> otherPararms)
        {
            foreach (var item in otherPararms)
            {
                this.parameters.Add(item.Key, item.Value);
            }
        }



        ///// <summary>
        ///// Initializes a new Custom Expression instance of the <see cref="ExpressionClip"/> class.
        ///// </summary>
        ///// <param name="sql">The SQL.</param>
        ///// <param name="type">The type.</param>
        ///// <param name="paramNames">The param names.</param>
        ///// <param name="types">The types.</param>
        ///// <param name="values">The values.</param>
        //public ExpressionClip(string sql, DbType type, string[] paramNames, DbType[] types, object[] values)
        //{
        //    Check.Require(!string.IsNullOrEmpty(sql), "sql could not be null or empty!");
        //    Check.Require(paramNames == null ||
        //        (types != null && values != null && paramNames.Length == types.Length && paramNames.Length == values.Length), 
        //        "length of paramNames, types and values must equal!");

        //    this.sql.Append(sql);
        //    this.dbType = type;
        //    if (paramNames != null)
        //    {
        //        for (int i = 0; i < paramNames.Length; ++i)
        //        {
        //            this.parameters.Add(paramNames[i], new KeyValuePair<DbType, object>(types[i], values[i]));
        //        }
        //    }
        //}

        /// <summary>
        /// 追加表达式
        /// </summary>
        /// <param name="op">操作类型</param>
        /// <param name="right">右边的表达式</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Append(QueryOperator op, ExpressionClip right)
        {
            this.sql.Append(SqlQueryUtils.ToString(op));
            this.sql.Append(SqlQueryUtils.ToString(right));
            SqlQueryUtils.AddParameters(this.parameters, right);
            return this;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>作为此实例副本的新对象。</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public virtual object Clone()
        {
            ExpressionClip newExpr = new ExpressionClip();
            newExpr.dbType = this.dbType;
            string tempSql = this.sql.ToString();

            Dictionary<string, KeyValuePair<DbType, object>>.Enumerator en = this.parameters.GetEnumerator();
            while (en.MoveNext())
            {
                object value = en.Current.Value.Value;
                if (value != null && value != DBNull.Value && value is ICloneable)
                {
                    value = ((ICloneable)value).Clone();
                }

                string newParamName = MakeUniqueParamNameWithoutPrefixToken();
                tempSql = tempSql.Replace('@' + en.Current.Key, '@' + newParamName);
                newExpr.Parameters.Add(newParamName, new KeyValuePair<DbType, object>(en.Current.Value.Key, value));
            }
            newExpr.sql.Append(tempSql);
            return newExpr;
        }

        #endregion

        #region Operators & Database Functions

        /// <summary>
        /// 判断表达式是否为空
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns><c>是</c> if [is null or empty] [the specified expr]; otherwise, <c>否</c>.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static bool IsNullOrEmpty(ExpressionClip expr)
        {
            return ((object)expr) == null || expr.sql.Length == 0;
        }

        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Alias(string aliasName)
        {
            Check.Require(aliasName, "aliasName", Check.NotNullOrEmpty);

            ExpressionClip expr = (ExpressionClip)this.Clone();
            if (expr.parameters.Count > 0)
                expr.Sql = "(" + expr.Sql + ")";
            expr.sql.Append(' ');
            SqlQueryUtils.AppendColumnName(expr.sql, aliasName);

            return expr;
        }

        #region String Functions

        /// <summary>
        /// where 表达式 Like
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>WhereClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public WhereClip Like(string right)
        {
            Check.Require(right != null, "right could not be null.");

            return new WhereClip().And(this, QueryOperator.Like, new ExpressionClip(this.DbType, right));
        }

        /// <summary>
        /// 包含操作
        /// </summary>
        /// <param name="subString">The sub string.</param>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public WhereClip Contains(string subString)
        {
            Check.Require(!string.IsNullOrEmpty(subString), "subString could not be null or empty!");

            return new WhereClip().And(this, QueryOperator.Like, new ExpressionClip(this.dbType, '%' + subString.Replace("%", "[%]").Replace("_", "[_]") + '%'));
        }

        /// <summary>
        /// 以pre
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>WhereClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public WhereClip StartsWith(string prefix)
        {
            Check.Require(!string.IsNullOrEmpty(prefix), "prefix could not be null or empty!");

            return new WhereClip().And(this, QueryOperator.Like, new ExpressionClip(this.dbType, prefix.Replace("%", "[%]").Replace("_", "[_]") + '%'));
        }

        /// <summary>
        /// Endses the with.
        /// </summary>
        /// <param name="suffix">The suffix.</param>
        /// <returns>WhereClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public WhereClip EndsWith(string suffix)
        {
            Check.Require(!string.IsNullOrEmpty(suffix), "suffix could not be null or empty!");

            return new WhereClip().And(this, QueryOperator.Like, new ExpressionClip(this.dbType, '%' + suffix.Replace("%", "[%]").Replace("_", "[_]")));
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Length
        {
            get
            {
                ExpressionClip expr = (ExpressionClip)this.Clone();
                expr.Sql = ColumnFormatter.Length(this.Sql);
                expr.dbType = DbType.Int32;

                return expr;
            }
        }

        /// <summary>
        /// To the upper.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip ToUpper()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.ToUpper(this.Sql);

            return expr;
        }

        /// <summary>
        /// To the lower.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip ToLower()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.ToLower(this.Sql);

            return expr;
        }

        /// <summary>
        /// Trims this instance.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Trim()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Trim(this.Sql);

            return expr;
        }

        /// <summary>
        /// Subs the string.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip SubString(int start)
        {
            Check.Require(start >= 0, "start must >= 0!");

            ExpressionClip expr = (ExpressionClip)this.Clone();
            ExpressionClip cloneExpr = (ExpressionClip)this.Clone();
            StringBuilder sb = new StringBuilder("SUBSTRING(");
            SqlQueryUtils.AppendColumnName(sb, this.Sql);
            sb.Append(',');
            sb.Append(start + 1);
            sb.Append(",LEN(");
            SqlQueryUtils.AppendColumnName(sb, cloneExpr.Sql);
            sb.Append(')');
            sb.Append(')');

            expr.sql = sb;
            SqlQueryUtils.AddParameters(expr.parameters, cloneExpr);

            return expr;
        }

        /// <summary>
        /// Subs the string.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip SubString(int start, int length)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.SubString(this.Sql, start, length);
            return expr;
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="subString">The sub string.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip IndexOf(string subString)
        {
            Check.Require(!string.IsNullOrEmpty(subString), "subString could not be null or empty!");

            ExpressionClip expr = (ExpressionClip)this.Clone();
            StringBuilder sb = new StringBuilder();
            sb.Append("CHARINDEX(");
            string paramName = MakeUniqueParamNameWithoutPrefixToken();
            sb.Append(paramName);
            sb.Append(',');
            SqlQueryUtils.AppendColumnName(sb, this.Sql);
            sb.Append(')');
            sb.Append("-1");

            expr.sql = sb;
            expr.dbType = DbType.Int32;
            expr.parameters.Add(paramName, new KeyValuePair<DbType, object>(this.dbType, subString));

            return expr;
        }

        /// <summary>
        /// Replaces the specified sub string.
        /// </summary>
        /// <param name="subString">The sub string.</param>
        /// <param name="replaceString">The replace string.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Replace(string subString, string replaceString)
        {
            Check.Require(!string.IsNullOrEmpty(subString), "subString could not be null or empty!");
            Check.Require(!string.IsNullOrEmpty(replaceString), "replaceString could not be null or empty!");

            ExpressionClip expr = (ExpressionClip)this.Clone();
            StringBuilder sb = new StringBuilder();
            sb.Append("REPLACE(");
            SqlQueryUtils.AppendColumnName(sb, this.Sql);
            sb.Append(',');
            string paramName = MakeUniqueParamNameWithoutPrefixToken();
            sb.Append(paramName);
            sb.Append(',');
            string paramName2 = MakeUniqueParamNameWithoutPrefixToken();
            sb.Append(paramName2);
            sb.Append(')');

            expr.sql = sb;
            expr.dbType = DbType.Int32;
            expr.parameters.Add(paramName, new KeyValuePair<DbType, object>(this.dbType, subString));
            expr.Parameters.Add(paramName2, new KeyValuePair<DbType, object>(this.dbType, replaceString));

            return expr;
        }

        #endregion

        #region DateTime Functions

        /// <summary>
        /// Gets the year.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip GetYear()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.DatePart(this.Sql, ColumnFormatter.DatePartType.Year);
            expr.dbType = DbType.Int32;

            return expr;
        }

        /// <summary>
        /// Gets the month.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip GetMonth()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.DatePart(this.Sql, ColumnFormatter.DatePartType.Month);
            expr.dbType = DbType.Int32;

            return expr;
        }

        /// <summary>
        /// Gets the day.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip GetDay()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.DatePart(this.Sql, ColumnFormatter.DatePartType.Day);
            expr.dbType = DbType.Int32;

            return expr;
        }

        /// <summary>
        /// Gets the current date.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip GetCurrentDate()
        {
            ExpressionClip expr = new ExpressionClip();
            expr.Sql = ColumnFormatter.GetCurrentDate();
            expr.dbType = DbType.DateTime;
            return expr;
        }

        /// <summary>
        /// Gets the current UTC date.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip GetCurrentUtcDate()
        {
            ExpressionClip expr = new ExpressionClip();
            expr.Sql = ColumnFormatter.GetCurrentUtcDate();
            expr.dbType = DbType.DateTime;
            return expr;
        }

        #endregion

        #region Aggregation Functions

        /// <summary>
        /// Gets the distinct.
        /// </summary>
        /// <value>The distinct.</value>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Distinct
        {
            get
            {
                ExpressionClip expr = (ExpressionClip)this.Clone();
                expr.Sql = "DISTINCT " + this.Sql;

                return expr;
            }
        }

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Count()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Count(this.Sql);
            expr.dbType = DbType.Int32;

            return expr;
        }

        /// <summary>
        /// Counts the specified is distinct.
        /// </summary>
        /// <param name="isDistinct">if set to <c>是</c> [is distinct].</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Count(bool isDistinct)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Count(this.Sql, isDistinct);
            expr.dbType = DbType.Int32;

            return expr;
        }

        /// <summary>
        /// Sums this instance.
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Sum()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Sum(this.Sql);

            return expr;
        }

        /// <summary>
        /// 最小值函数
        /// min(coloum)
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Min()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Min(this.Sql);

            return expr;
        }

        /// <summary>
        /// 最大值函数
        /// max(coloum)
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Max()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Max(this.Sql);
            return expr;
        }

        /// <summary>
        /// 平均值函数
        /// avg(coloum)
        /// </summary>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip Avg()
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Sql = ColumnFormatter.Avg(this.Sql);

            return expr;
        }

        #endregion

        #region Equals and Not Equals

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">与当前的 <see cref="T:System.Object" /> 进行比较的 <see cref="T:System.Object" />。</param>
        /// <returns><c>是</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>否</c>.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// ==
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator ==(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where.And(left, QueryOperator.IsNULL, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where.And(right, QueryOperator.IsNULL, null);
            }
            else
            {
                where.And(left, QueryOperator.Equal, right);
            }

            return where;
        }

        /// <summary>
        /// !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator !=(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where = where.And(left, QueryOperator.IsNULL, null).Not();
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where = where.And(right, QueryOperator.IsNULL, null).Not();
            }
            else
            {
                where.And(left, QueryOperator.NotEqual, right);
            }

            return where;
        }

        /// <summary>
        ///  ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator ==(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where.And(left, QueryOperator.IsNULL, null);
            }
            else
            {
                where.And(left, QueryOperator.Equal,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        /// <summary>
        /// !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator !=(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where = where.And(left, QueryOperator.IsNULL, null).Not();
            }
            else
            {
                where.And(left, QueryOperator.NotEqual,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        /// <summary>
        ///  ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator ==(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where.And(right, QueryOperator.IsNULL, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.Equal, right);
            }

            return where;
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator !=(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where = where.And(right, QueryOperator.IsNULL, null).Not();
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.NotEqual, right);
            }

            return where;
        }

        /// <summary>
        /// Ins the specified objs.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <returns>WhereClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public WhereClip In(params object[] objs)
        {
            Check.Require(objs != null && objs.Length > 0, "objs could not be null or empty.");

            WhereClip where = new WhereClip();
            if (objs.Length == 1 && !(objs[0] is string))
            {

                var s = objs[0] as IList;
                if (s != null && s.Count > 0)
                {
                    foreach (object obj in s)
                    {
                        where.Or(this == obj);
                    }
                }

            }
            else
            {
                foreach (object obj in objs)
                {
                    where.Or(this == obj);
                }
            }

            return where;
        }

        /// <summary>
        /// Ins the specified sub query.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>WhereClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public WhereClip In(ExpressionClip subQuery)
        {
            Check.Require(!ExpressionClip.IsNullOrEmpty(subQuery), "subQuery could not be null or empty.");

            WhereClip where = new WhereClip();
            where.Sql = string.Format("{0} IN ({1})", this.ToString(), subQuery.ToString());
            if (subQuery.Parameters.Count > 0)
            {
                Dictionary<string, KeyValuePair<DbType, object>>.Enumerator en = subQuery.Parameters.GetEnumerator();
                while (en.MoveNext())
                {
                    where.Parameters.Add(en.Current.Key, new KeyValuePair<DbType, object>(en.Current.Value.Key, en.Current.Value.Value));
                }
            }
            return where;
        }

        #endregion

        #region Greater and Less

        /// <summary>
        /// Implements the &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator >(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where.And(left, QueryOperator.Greater, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where.And(right, QueryOperator.Less, null);
            }
            else
            {
                where.And(left, QueryOperator.Greater, right);
            }

            return where;
        }

        /// <summary>
        /// Implements the &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator <(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where = where.And(left, QueryOperator.Less, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where = where.And(right, QueryOperator.Greater, null);
            }
            else
            {
                where.And(left, QueryOperator.Less, right);
            }

            return where;
        }

        /// <summary>
        /// Implements the &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator >(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where.And(left, QueryOperator.Greater, null);
            }
            else
            {
                where.And(left, QueryOperator.Greater,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        /// <summary>
        /// Implements the &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator <(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where = where.And(left, QueryOperator.Less, null);
            }
            else
            {
                where.And(left, QueryOperator.Less,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        /// <summary>
        /// Implements the &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator >(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where.And(right, QueryOperator.Less, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.Greater, right);
            }

            return where;
        }

        /// <summary>
        /// Implements the &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator <(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where = where.And(right, QueryOperator.Greater, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.Less, right);
            }

            return where;
        }

        #endregion

        #region Greater or Equals and Less or Equals

        /// <summary>
        /// Implements the &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator >=(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where.And(left, QueryOperator.GreaterOrEqual, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where.And(right, QueryOperator.LessOrEqual, null);
            }
            else
            {
                where.And(left, QueryOperator.GreaterOrEqual, right);
            }

            return where;
        }

        /// <summary>
        /// Implements the &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator <=(ExpressionClip left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (ExpressionClip.IsNullOrEmpty(right))
            {
                where = where.And(left, QueryOperator.LessOrEqual, null);
            }
            else if (ExpressionClip.IsNullOrEmpty(left))
            {
                where = where.And(right, QueryOperator.GreaterOrEqual, null);
            }
            else
            {
                where.And(left, QueryOperator.LessOrEqual, right);
            }

            return where;
        }

        /// <summary>
        /// Implements the &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator >=(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where.And(left, QueryOperator.GreaterOrEqual, null);
            }
            else
            {
                where.And(left, QueryOperator.GreaterOrEqual,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        /// <summary>
        /// Implements the &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator <=(ExpressionClip left, object right)
        {
            WhereClip where = new WhereClip();
            if (right == null || right == DBNull.Value)
            {
                where = where.And(left, QueryOperator.LessOrEqual, null);
            }
            else
            {
                where.And(left, QueryOperator.LessOrEqual,
                    new ExpressionClip(left.DbType, right));
            }

            return where;
        }

        /// <summary>
        /// Implements the &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator >=(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where.And(right, QueryOperator.LessOrEqual, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.GreaterOrEqual, right);
            }

            return where;
        }

        /// <summary>
        /// Implements the &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static WhereClip operator <=(object left, ExpressionClip right)
        {
            WhereClip where = new WhereClip();
            if (left == null || left == DBNull.Value)
            {
                where = where.And(right, QueryOperator.GreaterOrEqual, null);
            }
            else
            {
                where.And(new ExpressionClip(right.DbType, left),
                    QueryOperator.LessOrEqual, right);
            }

            return where;
        }

        /// <summary>
        /// Betweens the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>WhereClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public WhereClip Between(object left, object right)
        {
            Check.Require(left != null, "left could not be null.");
            Check.Require(right != null, "right could not be null.");

            return (this >= left).And(this <= right);
        }

        #endregion

        #region + - * / %

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator +(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Add, right);
            return expr;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator +(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Add, right);
            return expr;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator +(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Add,
                new ExpressionClip(left.dbType, right));
            return expr;
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator -(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Subtract, right);
            return expr;
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator -(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Subtract, right);
            return expr;
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator -(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Subtract,
                new ExpressionClip(left.dbType, right));
            return expr;
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator *(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Multiply, right);
            return expr;
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator *(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Multiply, right);
            return expr;
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator *(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Multiply,
                new ExpressionClip(left.dbType, right));
            return expr;
        }

        /// <summary>
        /// Implements the /.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator /(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Divide, right);
            return expr;
        }

        /// <summary>
        /// Implements the /.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator /(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Divide, right);
            return expr;
        }

        /// <summary>
        /// Implements the /.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator /(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Divide,
                new ExpressionClip(left.dbType, right));
            return expr;
        }

        /// <summary>
        /// Implements the %.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator %(ExpressionClip left, ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Modulo, right);
            expr.dbType = DbType.Int32;
            return expr;
        }

        /// <summary>
        /// Implements the %.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator %(object left, ExpressionClip right)
        {
            ExpressionClip expr = new ExpressionClip(right.dbType, left);
            expr.Append(QueryOperator.Modulo, right);
            expr.dbType = DbType.Int32;
            return expr;
        }

        /// <summary>
        /// Implements the %.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator %(ExpressionClip left, object right)
        {
            ExpressionClip expr = (ExpressionClip)left.Clone();
            expr.Append(QueryOperator.Modulo,
                new ExpressionClip(left.dbType, right));
            expr.dbType = DbType.Int32;
            return expr;
        }

        #endregion

        #region Bitwise

        /// <summary>
        /// Implements the !.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public static ExpressionClip operator !(ExpressionClip left)
        {
            ExpressionClip expr = new ExpressionClip().Append(QueryOperator.BitwiseNOT,
                left);
            return expr;
        }

        /// <summary>
        /// Bitwises the and.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip BitwiseAnd(ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseAND,
                right);

            return expr;
        }

        /// <summary>
        /// Bitwises the and.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip BitwiseAnd(object right)
        {
            Check.Require(right != null, "right could not be null!");

            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseAND,
                new ExpressionClip(this.dbType, right));

            return expr;
        }

        /// <summary>
        /// Bitwises the or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip BitwiseOr(ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseOR,
                right);

            return expr;
        }

        /// <summary>
        /// Bitwises the or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip BitwiseOr(object right)
        {
            Check.Require(right != null, "right could not be null!");

            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseOR,
                new ExpressionClip(this.dbType, right));

            return expr;
        }

        /// <summary>
        /// Bitwises the x or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip BitwiseXOr(ExpressionClip right)
        {
            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseXOR,
                right);

            return expr;
        }

        /// <summary>
        /// Bitwises the x or.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>ExpressionClip.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-8</para>
        /// </remarks>
        public ExpressionClip BitwiseXOr(object right)
        {
            Check.Require(right != null, "right could not be null!");

            ExpressionClip expr = (ExpressionClip)this.Clone();
            expr.Append(QueryOperator.BitwiseXOR,
                new ExpressionClip(this.dbType, right));

            return expr;
        }

        #endregion

        #endregion
    }
}