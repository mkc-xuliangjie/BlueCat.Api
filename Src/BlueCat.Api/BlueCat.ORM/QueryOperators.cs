using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.ORM
{
    /// <summary>
    /// ��ѯ����
    /// </summary>
    /// <remarks>
    ///  	<para>������Teddy</para>
    ///  	<para>���ڣ�2016-9-7</para>
    /// </remarks>
    public enum QueryOperator
    {
        /// <summary>
        /// ����
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        Equal,
        /// <summary>
        /// ������
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        NotEqual,
        /// <summary>
        /// The greater
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        Greater,
        /// <summary>
        /// The less
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        Less,
        /// <summary>
        /// The greater or equal
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        GreaterOrEqual,
        /// <summary>
        /// The less or equal
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        LessOrEqual,
        /// <summary>
        /// The like
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        Like,
        /// <summary>
        /// The bitwise and
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        BitwiseAND,
        /// <summary>
        /// The bitwise or
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        BitwiseOR,
        /// <summary>
        /// The bitwise xor
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        BitwiseXOR,
        /// <summary>
        /// The bitwise not
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        BitwiseNOT,
        /// <summary>
        /// The is null
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        IsNULL,
        /// <summary>
        /// The add
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        Add,
        /// <summary>
        /// The subtract
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        Subtract,
        /// <summary>
        /// The multiply
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        Multiply,
        /// <summary>
        /// The divide
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        Divide,
        /// <summary>
        /// The modulo
        /// </summary>
        /// <remarks>
        ///  	<para>������Teddy</para>
        ///  	<para>���ڣ�2016-9-7</para>
        /// </remarks>
        Modulo,
    }
}
