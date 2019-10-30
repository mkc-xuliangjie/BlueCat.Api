using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace BlueCat.ORM
{
    /// <summary>
    /// 插入部分接口
    /// </summary>
    /// <remarks>
    ///  	<para>创建：Teddy</para>
    ///  	<para>日期：2016-9-7</para>
    /// </remarks>
    public interface IInsertSqlSection
    {
        /// <summary>
        /// 执行插入
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        int Execute();
        /// <summary>
        /// 设置事务
        /// </summary>
        /// <param name="tran">事务</param>
        /// <returns>返回插入部分</returns>
        /// <remarks>
        ///  	<para>创建：Teddy</para>
        ///  	<para>日期：2016-9-7</para>
        /// </remarks>
        IInsertSqlSection SetTransaction(DbTransaction tran);
        


    }
}
