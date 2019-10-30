using System;
using System.Data;
using System.Data.Common;

namespace BlueCat.ORM
{
    /// <summary>
    /// SQL 查询工程
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-9-7</para>
    /// </remarks>
    public interface ISqlQueryFactory
    {




        /// <summary>
        /// 创建删除命令
        /// </summary>
        /// <param name="tableName">删除数据的表名称</param>
        /// <param name="where">where 条件</param>
        /// <returns>命令</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateDeleteCommand(string tableName, WhereClip where);
        /// <summary>
        /// 创建插入命令
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columns">列名列表.</param>
        /// <param name="types">所对应的数据类型</param>
        /// <param name="values">每列对应的值</param>
        /// <returns>命令.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateInsertCommand(string tableName, string[] columns, DbType[] types, object[] values);
        

        /// <summary>
        /// 是否支持批量插入，支持批量插入调用CreateBatchInsertCommandText否则 循环调用CreateInsertCommand
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2017/8/23</para>
        /// </remarks>
        bool SupportBatchInsert();


        /// <summary>
        /// 获取批量插入文本
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnNames">列名列表</param>
        /// <param name="columnTypes">所对应的数据类型</param>
        /// <param name="rows">数据记录</param>
        /// <returns>命令字符串</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2017/8/23</para>
        /// </remarks>
        DbCommand CreateBatchInsertCommand(string tableName, string[] columnNames, DbType[] columnTypes, object[][] rows);


        /// <summary>
        /// 创建查询命令
        /// </summary>
        /// <param name="where">where条件</param>
        /// <param name="columns">列</param>
        /// <returns>命令.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateSelectCommand(WhereClip where, string[] columns);
        /// <summary>
        /// 创建更新命令
        /// </summary>
        /// <param name="tableName">名称</param>
        /// <param name="where">Where条件</param>
        /// <param name="columns">列集合</param>
        /// <param name="types">列所对应的类型</param>
        /// <param name="values">列所对应的值</param>
        /// <returns>命令</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateUpdateCommand(string tableName, WhereClip where, string[] columns, DbType[] types, object[] values);
        /// <summary>
        /// 创建分页命令
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="columns">查询的列</param>
        /// <param name="topCount">取的条数</param>
        /// <param name="skipCount">跳过的条数</param>
        /// <param name="identyColumn">主键列名称</param>
        /// <param name="identyColumnIsNumber">主键是否为数字</param>
        /// <returns>命令.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateSelectRangeCommand(WhereClip where, string[] columns, int topCount, int skipCount, string identyColumn, bool identyColumnIsNumber);
        /// <summary>
        /// 创建自定义命令
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramNames">The 参数 names.</param>
        /// <param name="paramTypes">The 参数 types.</param>
        /// <param name="paramValues">The 参数 values.</param>
        /// <returns>DbCommand.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateCustomSqlCommand(string sql, string[] paramNames, DbType[] paramTypes, object[] paramValues);

        /// <summary>
        /// 创建存储过程命令
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="paramNames">参数列表</param>
        /// <param name="paramTypes">数据类型列表</param>
        /// <param name="paramValues">参数对应的数据</param>
        /// <returns>DbCommand.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateStoredProcedureCommand(string procedureName, string[] paramNames, DbType[] paramTypes, object[] paramValues);
        /// <summary>
        /// 创建执行存储过程的命令
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="paramNames">参数名称列表</param>
        /// <param name="paramTypes">参数类型列表</param>
        /// <param name="paramValues">参数值列表</param>
        /// <param name="outParamNames">输出参数名列表</param>
        /// <param name="outParamTypes">初始参数类型列表</param>
        /// <param name="outParamSizes">参数列表大小</param>
        /// <returns>DbCommand.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateStoredProcedureCommand(string procedureName, string[] paramNames, DbType[] paramTypes, object[] paramValues, 
            string[] outParamNames, DbType[] outParamTypes, int[] outParamSizes);
        /// <summary>
        /// Creates the stored procedure command.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The 参数 names.</param>
        /// <param name="paramTypes">The 参数 types.</param>
        /// <param name="paramValues">The 参数 values.</param>
        /// <param name="outParamNames">The out 参数 names.</param>
        /// <param name="outParamTypes">The out 参数 types.</param>
        /// <param name="outParamSizes">The out 参数 sizes.</param>
        /// <param name="inOutParamNames">The in out 参数 names.</param>
        /// <param name="inOutParamTypes">The in out 参数 types.</param>
        /// <param name="inOutParamSizes">The in out 参数 sizes.</param>
        /// <param name="inOutParamValues">The in out 参数 values.</param>
        /// <returns>DbCommand.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateStoredProcedureCommand(string procedureName, string[] paramNames, DbType[] paramTypes, object[] paramValues, 
            string[] outParamNames, DbType[] outParamTypes, int[] outParamSizes, 
            string[] inOutParamNames, DbType[] inOutParamTypes, int[] inOutParamSizes, object[] inOutParamValues);
        /// <summary>
        /// Creates the stored procedure command.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="paramNames">The 参数 names.</param>
        /// <param name="paramTypes">The 参数 types.</param>
        /// <param name="paramValues">The 参数 values.</param>
        /// <param name="outParamNames">The out 参数 names.</param>
        /// <param name="outParamTypes">The out 参数 types.</param>
        /// <param name="outParamSizes">The out 参数 sizes.</param>
        /// <param name="inOutParamNames">The in out 参数 names.</param>
        /// <param name="inOutParamTypes">The in out 参数 types.</param>
        /// <param name="inOutParamSizes">The in out 参数 sizes.</param>
        /// <param name="inOutParamValues">The in out 参数 values.</param>
        /// <param name="returnValueParamName">Name of the return value 参数.</param>
        /// <param name="returnValueParamType">Type of the return value 参数.</param>
        /// <param name="returnValueParamSize">Size of the return value 参数.</param>
        /// <returns>DbCommand.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        DbCommand CreateStoredProcedureCommand(string procedureName, string[] paramNames, DbType[] paramTypes, object[] paramValues, 
            string[] outParamNames, DbType[] outParamTypes, int[] outParamSizes, 
            string[] inOutParamNames, DbType[] inOutParamTypes, int[] inOutParamSizes, object[] inOutParamValues, 
            string returnValueParamName, DbType returnValueParamType, int returnValueParamSize);
    }
}
